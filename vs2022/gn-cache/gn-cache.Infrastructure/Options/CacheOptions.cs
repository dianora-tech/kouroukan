namespace GnCache.Infrastructure.Options;

/// <summary>
/// Options de configuration globales du systeme de cache.
/// Section de configuration : "Cache".
/// </summary>
public sealed class CacheOptions
{
    /// <summary>Cle de la section de configuration.</summary>
    public const string SectionName = "Cache";

    /// <summary>TTL par defaut du cache L1 MemoryCache en minutes. Defaut: 5.</summary>
    public int DefaultMemoryTtlMinutes { get; set; } = 5;

    /// <summary>TTL par defaut du cache L2 Redis en minutes. Defaut: 60.</summary>
    public int DefaultRedisTtlMinutes { get; set; } = 60;

    /// <summary>Chemin du repertoire contenant les fichiers JSON seed. Defaut: "data/seed".</summary>
    public string SeedDataPath { get; set; } = "data/seed";

    /// <summary>URL de base de la gateway API pour le rechargement. Ex: "http://api-gateway:5000".</summary>
    public string ApiGatewayBaseUrl { get; set; } = string.Empty;

    /// <summary>Prefixe des cles Redis. Defaut: "gn-cache:".</summary>
    public string RedisKeyPrefix { get; set; } = "gn-cache:";

    /// <summary>Activer le fallback L1 si Redis est indisponible. Defaut: true.</summary>
    public bool EnableL1Fallback { get; set; } = true;
}
