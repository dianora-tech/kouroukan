using System.Text;
using GnDapper.Extensions;
using GnMessaging.Extensions;
using GnValidation.Extensions;
using Presences.Api.Authorization;
using Presences.Api.Middleware;
using Presences.Application.Handlers;
using Presences.Domain.Ports.Input;
using Presences.Domain.Ports.Output;
using Presences.Domain.Services;
using Presences.Infrastructure.Repositories;
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
        .Enrich.WithProperty("Application", "presences-service"));

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
        typeof(Presences.Application.Validators.CreateAppelValidator).Assembly);
    builder.Services.AddGnValidationPipeline();

    // 7. GnMessaging (RabbitMQ publisher)
    builder.Services.AddGnMessaging(builder.Configuration);

    // 8. MediatR
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(AppelCommandHandler).Assembly));

    // 9. Domain Services
    builder.Services.AddScoped<IAppelService, AppelService>();
    builder.Services.AddScoped<IAbsenceService, AbsenceService>();
    builder.Services.AddScoped<IBadgeageService, BadgeageService>();

    // 10. Repositories
    builder.Services.AddScoped<IAppelRepository, AppelRepository>();
    builder.Services.AddScoped<IAbsenceRepository, AbsenceRepository>();
    builder.Services.AddScoped<IBadgeageRepository, BadgeageRepository>();

    // 11. Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Kouroukan - Presences Service",
            Version = "v1",
            Description = "Microservice de gestion des appels, absences, justifications et badgeage NFC/QR Code."
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

    Log.Information("Presences Service demarre sur le port 5004");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Le service Presences a echoue au demarrage");
}
finally
{
    Log.CloseAndFlush();
}
