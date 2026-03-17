using System.Text;
using GnDapper.Extensions;
using GnMessaging.Extensions;
using GnValidation.Extensions;
using Pedagogie.Api.Authorization;
using Pedagogie.Api.Middleware;
using Pedagogie.Application.Handlers;
using Pedagogie.Domain.Ports.Input;
using Pedagogie.Domain.Ports.Output;
using Pedagogie.Domain.Services;
using Pedagogie.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 1. Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .WriteTo.Console()
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "pedagogie-service"));

    // 2. CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(
                    "https://www.kouroukan.gn",
                    "https://app.kouroukan.gn",
                    "http://localhost:3000",
                    "http://localhost:3001")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // 3. JWT Authentication (validation only — never generates tokens)
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtSection = builder.Configuration.GetSection("Jwt");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });

    // 4. Authorization RBAC — Dynamic permission policy provider
    builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
    builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
    builder.Services.AddAuthorization();

    // 5. GnDapper (Data Access Layer — PostgreSQL via Npgsql with Audit)
    builder.Services.AddDataAccessLayerWithAudit(builder.Configuration);

    // 6. GnValidation (FluentValidation auto-discovery + MediatR pipeline)
    builder.Services.AddGnValidation(builder.Configuration,
        typeof(Pedagogie.Application.Validators.CreateNiveauClasseValidator).Assembly);
    builder.Services.AddGnValidationPipeline();

    // 7. GnMessaging (RabbitMQ publisher)
    builder.Services.AddGnMessaging(builder.Configuration);

    // 8. MediatR
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(NiveauClasseCommandHandler).Assembly));

    // 9. Domain Services
    builder.Services.AddScoped<INiveauClasseService, NiveauClasseService>();
    builder.Services.AddScoped<IClasseService, ClasseService>();
    builder.Services.AddScoped<IMatiereService, MatiereService>();
    builder.Services.AddScoped<ISalleService, SalleService>();
    builder.Services.AddScoped<ISeanceService, SeanceService>();
    builder.Services.AddScoped<ICahierTextesService, CahierTextesService>();

    // 10. Repositories
    builder.Services.AddScoped<INiveauClasseRepository, NiveauClasseRepository>();
    builder.Services.AddScoped<IClasseRepository, ClasseRepository>();
    builder.Services.AddScoped<IMatiereRepository, MatiereRepository>();
    builder.Services.AddScoped<ISalleRepository, SalleRepository>();
    builder.Services.AddScoped<ISeanceRepository, SeanceRepository>();
    builder.Services.AddScoped<ICahierTextesRepository, CahierTextesRepository>();

    // 11. Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Kouroukan - Pedagogie Service",
            Version = "v1",
            Description = "Microservice de gestion des classes, niveaux, matieres, emplois du temps, salles et cahier de textes."
        });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    // --- Pipeline ---
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseCors();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Pedagogie Service demarre sur le port 5002");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Le service Pedagogie a echoue au demarrage");
}
finally
{
    Log.CloseAndFlush();
}
