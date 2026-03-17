namespace GnCache.Domain;

/// <summary>
/// Enregistrement declaratif d'une entite a mettre en cache.
/// Chaque entite est definie par sa cle, son type, son API source et son cron.
/// </summary>
public sealed record CacheEntityRegistration
{
    /// <summary>Cle unique du cache (ex: "regions", "niveaux-classes").</summary>
    public required string CacheKey { get; init; }

    /// <summary>Type CLR des donnees (ex: typeof(Region)).</summary>
    public required Type EntityType { get; init; }

    /// <summary>URL de l'API source pour le rechargement (ex: "/api/geo/regions").</summary>
    public required string SourceApiUrl { get; init; }

    /// <summary>Expression cron Quartz a 6 champs (ex: "0 0 */12 * * ?").</summary>
    public required string CronExpression { get; init; }

    /// <summary>Nom du fichier JSON seed (ex: "regions.json").</summary>
    public required string SeedFileName { get; init; }

    /// <summary>TTL du cache L2 Redis. Defaut: 1 heure.</summary>
    public TimeSpan RedisTtl { get; init; } = TimeSpan.FromHours(1);

    /// <summary>TTL du cache L1 MemoryCache. Defaut: 5 minutes.</summary>
    public TimeSpan MemoryTtl { get; init; } = TimeSpan.FromMinutes(5);
}
