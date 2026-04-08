using System.Text;
using System.Threading.RateLimiting;
using GnDapper.Extensions;
using GnSecurity.Extensions;
using GnSecurity.Jwt;
using GnSecurity.Rbac;
using Kouroukan.Api.Gateway.Auth;
using Kouroukan.Api.Gateway.Services;
using Kouroukan.Api.Gateway.HealthChecks;
using Kouroukan.Api.Gateway.Middleware;
using Minio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

// --- Configuration Serilog ---
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
        .Enrich.WithProperty("Application", "kouroukan-api-gateway"));

    // 2. CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(
                    "https://www.kouroukan.gn",
                    "https://app.kouroukan.gn",
                    "https://kouroukan.dianora.org",
                    "https://app.kouroukan.dianora.org",
                    "http://localhost",
                    "http://localhost:3000",
                    "http://localhost:3001",
                    "http://localhost:8080")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // 3. GnSecurity (JWT + RBAC + Hashing + Encryption)
    builder.Services.AddGnSecurity(builder.Configuration);

    // Implementations requises par GnSecurity
    builder.Services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
    builder.Services.AddScoped<IUserClaimsProvider, UserClaimsProvider>();
    builder.Services.AddScoped<IPermissionStore, PermissionStore>();

    // Service de token applicatif
    builder.Services.AddScoped<ITokenService, TokenService>();

    // Service de gestion des utilisateurs
    builder.Services.AddScoped<IUserService, UserService>();

    // Service d'envoi d'emails transactionnels
    builder.Services.AddSingleton<IEmailService, EmailService>();

    // Cloudflare Turnstile (anti-bot)
    builder.Services.AddHttpClient<TurnstileService>();
    builder.Services.AddScoped<ITurnstileService, TurnstileService>();

    // Services d'administration de la plateforme
    builder.Services.AddHttpClient<NimbaSmsService>();
    builder.Services.AddScoped<IAdminService, AdminService>();
    builder.Services.AddScoped<ILiaisonEnseignantService, LiaisonEnseignantService>();
    builder.Services.AddScoped<IQrCodeService, QrCodeService>();
    builder.Services.AddScoped<IForfaitUserService, ForfaitUserService>();

    // MinIO (stockage fichiers S3-compatible)
    builder.Services.AddSingleton<IMinioClient>(sp =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var endpoint = config["MinIO:Endpoint"] ?? "minio:9000";
        var accessKey = config["MinIO:AccessKey"] ?? "kouroukan";
        var secretKey = config["MinIO:SecretKey"] ?? "kouroukan";
        return new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .Build();
    });
    builder.Services.AddSingleton<IMinioStorageService, MinioStorageService>();

    // 4. GnDapper (Data Access Layer — PostgreSQL via Npgsql)
    builder.Services.AddDataAccessLayer(builder.Configuration);

    // 5. JWT Authentication
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

    // 6. Authorization RBAC — Dynamic policy provider
    builder.Services.AddSingleton<IAuthorizationPolicyProvider, RbacPolicyProvider>();
    builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    builder.Services.AddAuthorization();

    // 7. Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Global : 100 requetes par minute par IP
        options.AddFixedWindowLimiter("global", limiterOptions =>
        {
            limiterOptions.PermitLimit = 100;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 0;
        });

        // Auth : 10 requetes par minute sur /api/auth/login (anti-brute-force)
        options.AddFixedWindowLimiter("auth", limiterOptions =>
        {
            limiterOptions.PermitLimit = 10;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 0;
        });

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1)
                }));
    });

    // 8. YARP Reverse Proxy
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    // 9. Redis Distributed Cache
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "kouroukan-gateway:";
    });

    // 10. Health Checks
    builder.Services.AddHttpClient();
    builder.Services.AddHealthChecks()
        .AddCheck<PostgreSqlHealthCheck>("postgresql", tags: ["infrastructure"])
        .AddCheck<RedisHealthCheck>("redis", tags: ["infrastructure"])
        .AddCheck<OllamaHealthCheck>("ollama", tags: ["infrastructure"])
        .AddDownstreamServices(builder.Configuration);

    // 11. Swagger avec Bearer
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Kouroukan API Gateway",
            Version = "v1",
            Description = "Point d'entree unique du backend Kouroukan. Gere l'authentification, l'autorisation et le routage vers les microservices."
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Entrer le token JWT : Bearer {token}"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    // --- Pipeline Middleware ---

    // Correlation ID (toujours en premier)
    app.UseMiddleware<CorrelationIdMiddleware>();

    // Logging des requetes
    app.UseMiddleware<RequestLoggingMiddleware>();

    // Gestion globale des exceptions
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // Rate Limiting
    app.UseRateLimiter();

    // CORS
    app.UseCors();

    // Swagger (dev uniquement)
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Kouroukan API Gateway v1");
        });
    }

    // HTTPS Redirection
    app.UseHttpsRedirection();

    // Authentication
    app.UseAuthentication();

    // CGU Check (apres Authentication, avant Authorization)
    app.UseMiddleware<CguCheckMiddleware>();

    // Authorization
    app.UseAuthorization();

    // Endpoints
    app.MapControllers();
    app.MapReverseProxy();

    // Version endpoint (public, pas d'auth)
    var appVersion = Environment.GetEnvironmentVariable("APP_VERSION") ?? "v1.0-dev";
    app.MapGet("/api/version", () => Results.Ok(new { version = appVersion }))
        .AllowAnonymous()
        .WithTags("System");

    // Header X-App-Version sur toutes les reponses
    app.Use(async (context, next) =>
    {
        context.Response.Headers["X-App-Version"] = appVersion;
        await next();
    });

    Log.Information("Kouroukan API Gateway {Version} demarre sur le port 5000", appVersion);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "L'application a echoue au demarrage");
}
finally
{
    Log.CloseAndFlush();
}
