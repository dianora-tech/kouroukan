using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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
            // Remplacer la connection string par celle du conteneur PostgreSQL
            services.Configure<ConnectionStringOptions>(options =>
            {
                options.DefaultConnection = _dbFixture.ConnectionString;
            });

            // Ajouter l'authentification de test
            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
        });

        builder.UseEnvironment("Testing");
    }
}

/// <summary>
/// Options de connection string pour l'injection de dependances.
/// </summary>
public class ConnectionStringOptions
{
    public string DefaultConnection { get; set; } = string.Empty;
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
