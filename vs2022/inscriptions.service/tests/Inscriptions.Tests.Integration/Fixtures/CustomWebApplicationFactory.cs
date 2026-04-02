using System.Security.Claims;
using System.Text.Encodings.Web;
using GnDapper.Connection;
using GnDapper.Options;
using GnMessaging.Abstractions;
using GnMessaging.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Inscriptions.Tests.Integration.Fixtures;

/// <summary>
/// Factory WebApplication pour les tests d'integration.
/// Configure un conteneur PostgreSQL ephemere et l'authentification de test.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlFixture _dbFixture;

    public CustomWebApplicationFactory(PostgreSqlFixture dbFixture)
    {
        _dbFixture = dbFixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remplacer la connection string GnDapper par celle du conteneur PostgreSQL
            services.Configure<GnDapperOptions>(options =>
            {
                options.ConnectionString = _dbFixture.ConnectionString;
            });

            // Re-enregistrer le factory avec la nouvelle connection string
            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory>(sp =>
            {
                var opts = sp.GetRequiredService<IOptions<GnDapperOptions>>();
                return new NpgsqlConnectionFactory(opts);
            });

            // Remplacer RabbitMQ par un stub (pas de broker en CI)
            services.RemoveAll<IMessagePublisher>();
            services.AddSingleton<IMessagePublisher, NullMessagePublisher>();

            // Supprimer les hosted services et autres singletons RabbitMQ
            services.RemoveAll<IHostedService>();
            services.RemoveAll<IMessageConsumer>();

            // Ajouter l'authentification de test
            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
        });

        builder.UseEnvironment("Testing");
    }
}

/// <summary>
/// Stub IMessagePublisher qui ne fait rien — pas de connexion RabbitMQ.
/// </summary>
internal sealed class NullMessagePublisher : IMessagePublisher
{
    public Task PublishAsync<T>(T message, string exchange, string routingKey,
        PublishOptions? options = null, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        return Task.CompletedTask;
    }
}

/// <summary>
/// Handler d'authentification de test — simule un utilisateur avec les permissions admin.
/// </summary>
public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public static string DefaultRole { get; set; } = "super_admin";
    public static string[] DefaultPermissions { get; set; } = [
        "inscriptions:read", "inscriptions:create", "inscriptions:update", "inscriptions:delete"
    ];

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Pas de header Authorization → echec
        if (!Context.Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = Context.Request.Headers.Authorization.ToString();

        // Token "invalid" → echec explicite
        if (token.Contains("invalid"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Token invalide"));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Name, "Test User"),
            new(ClaimTypes.Email, "test@kouroukan.gn"),
            new("role", DefaultRole),
        };

        foreach (var permission in DefaultPermissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        // Ajouter la version CGU si specifie
        if (!string.IsNullOrEmpty(CguVersion))
        {
            claims.Add(new Claim("cgu_version", CguVersion));
        }

        var identity = new ClaimsIdentity(claims, "TestScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <summary>
    /// Version CGU acceptee par l'utilisateur de test.
    /// Null = CGU non acceptees.
    /// </summary>
    public static string? CguVersion { get; set; } = "1.0.0";
}
