using GnSecurity.Encryption;
using GnSecurity.Hashing;
using GnSecurity.Jwt;
using GnSecurity.Rbac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GnSecurity.Extensions;

/// <summary>
/// Extensions pour l'enregistrement des services GnSecurity dans le conteneur DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre tous les services GnSecurity dans le conteneur d'injection de dependances.
    /// <para>
    /// Services enregistres :
    /// <list type="bullet">
    ///   <item><description><see cref="IPasswordHasher"/> → <see cref="Argon2PasswordHasher"/> (Singleton)</description></item>
    ///   <item><description><see cref="IJwtTokenService"/> → <see cref="JwtTokenService"/> (Singleton)</description></item>
    ///   <item><description><see cref="IRefreshTokenService"/> → <see cref="RefreshTokenService"/> (Scoped)</description></item>
    ///   <item><description><see cref="IRbacService"/> → <see cref="RbacService"/> (Scoped)</description></item>
    ///   <item><description><see cref="IAesEncryptionService"/> → <see cref="AesEncryptionService"/> (Singleton)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Le consommateur doit enregistrer ses propres implementations de :
    /// <list type="bullet">
    ///   <item><description><see cref="IRefreshTokenStore"/> — persistance des refresh tokens</description></item>
    ///   <item><description><see cref="IUserClaimsProvider"/> — recuperation des claims utilisateur</description></item>
    ///   <item><description><see cref="IPermissionStore"/> — persistance des roles et permissions</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <param name="configuration">Configuration de l'application (section "Jwt" attendue).</param>
    /// <returns>La collection de services pour le chainage.</returns>
    public static IServiceCollection AddGnSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        // Configuration JWT
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        // Hashing — Singleton (stateless, thread-safe)
        services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

        // JWT — Singleton (stateless une fois configure, thread-safe)
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // Refresh Token — Scoped (depend du store qui est probablement scoped via DbConnection)
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // RBAC — Scoped (depend du store scoped, le cache IMemoryCache est singleton en interne)
        services.AddScoped<IRbacService, RbacService>();

        // Encryption — Singleton (stateless, thread-safe)
        services.AddSingleton<IAesEncryptionService, AesEncryptionService>();

        // Memory Cache (si pas deja enregistre)
        services.AddMemoryCache();

        return services;
    }
}
