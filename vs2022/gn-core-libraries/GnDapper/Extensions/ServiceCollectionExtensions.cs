using Dapper;
using GnDapper.Connection;
using GnDapper.Options;
using GnDapper.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GnDapper.Extensions;

/// <summary>
/// Extensions pour <see cref="IServiceCollection"/> permettant d'enregistrer
/// les services de la couche d'acces aux donnees GnDapper.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre les services de base de la couche d'acces aux donnees :
    /// <list type="bullet">
    /// <item><see cref="IDbConnectionFactory"/> → <see cref="NpgsqlConnectionFactory"/> (Singleton)</item>
    /// <item><see cref="IRepository{T}"/> → <see cref="Repository{T}"/> (Scoped, generique ouvert)</item>
    /// </list>
    /// Configure le mapping Dapper snake_case ↔ PascalCase.
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <param name="configuration">Configuration de l'application.</param>
    /// <returns>La collection de services pour le chainage.</returns>
    public static IServiceCollection AddDataAccessLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        // Configuration
        services.Configure<GnDapperOptions>(configuration.GetSection("GnDapper"));

        // Mapping Dapper : PascalCase ↔ snake_case
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        // Connection Factory — Singleton (le pool Npgsql est interne)
        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

        // Repository generique — Scoped (une instance par requete HTTP)
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }

    /// <summary>
    /// Enregistre les services de la couche d'acces aux donnees avec support de l'audit
    /// et de la suppression logique :
    /// <list type="bullet">
    /// <item>Appelle <see cref="AddDataAccessLayer"/> pour les services de base.</item>
    /// <item>Ajoute <see cref="Microsoft.AspNetCore.Http.IHttpContextAccessor"/> pour recuperer l'utilisateur courant.</item>
    /// <item>Enregistre <see cref="AuditRepository{T}"/> comme implementation supplementaire.</item>
    /// </list>
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <param name="configuration">Configuration de l'application.</param>
    /// <returns>La collection de services pour le chainage.</returns>
    public static IServiceCollection AddDataAccessLayerWithAudit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        // Services de base
        services.AddDataAccessLayer(configuration);

        // HttpContextAccessor pour l'audit (recuperation de l'utilisateur courant)
        services.AddHttpContextAccessor();

        return services;
    }
}
