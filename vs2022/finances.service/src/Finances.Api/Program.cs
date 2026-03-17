using System.Text;
using Finances.Api.Authorization;
using Finances.Api.Middleware;
using Finances.Application.Handlers;
using Finances.Domain.Ports.Input;
using Finances.Domain.Ports.Output;
using Finances.Domain.Services;
using Finances.Infrastructure.Repositories;
using GnDapper.Extensions;
using GnMessaging.Extensions;
using GnValidation.Extensions;
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
        .Enrich.WithProperty("Application", "finances-service"));

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

    // 3. JWT Authentication (validation uniquement, jamais de generation)
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

    // 4. Authorization RBAC
    builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
    builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
    builder.Services.AddAuthorization();

    // 5. GnDapper (Data Access Layer with Audit)
    builder.Services.AddDataAccessLayerWithAudit(builder.Configuration);

    // 6. GnValidation (FluentValidation with MediatR pipeline)
    builder.Services.AddGnValidation(builder.Configuration,
        typeof(Finances.Application.Validators.CreateFactureValidator).Assembly);
    builder.Services.AddGnValidationPipeline();

    // 7. GnMessaging (RabbitMQ)
    builder.Services.AddGnMessaging(builder.Configuration);

    // 8. MediatR
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(FactureCommandHandler).Assembly));

    // 9. Domain Services
    builder.Services.AddScoped<IFactureService, FactureService>();
    builder.Services.AddScoped<IPaiementService, PaiementService>();
    builder.Services.AddScoped<IDepenseService, DepenseService>();
    builder.Services.AddScoped<IRemunerationEnseignantService, RemunerationEnseignantService>();

    // 10. Repositories
    builder.Services.AddScoped<IFactureRepository, FactureRepository>();
    builder.Services.AddScoped<IPaiementRepository, PaiementRepository>();
    builder.Services.AddScoped<IDepenseRepository, DepenseRepository>();
    builder.Services.AddScoped<IRemunerationEnseignantRepository, RemunerationEnseignantRepository>();

    // 11. Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Kouroukan - Finances Service",
            Version = "v1",
            Description = "Microservice de gestion financiere : facturation, paiements (Mobile Money + especes), depenses et remuneration enseignants."
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

    // 12. HttpContextAccessor
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    // Middleware pipeline
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

    Log.Information("Finances Service demarre sur le port 5005");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Le service Finances a echoue au demarrage");
}
finally
{
    Log.CloseAndFlush();
}
