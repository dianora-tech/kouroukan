namespace GnCache.Domain;

/// <summary>
/// Source de chargement des donnees du cache.
/// </summary>
public enum CacheSource
{
    /// <summary>Chargement depuis un fichier JSON de seed.</summary>
    JsonSeed,

    /// <summary>Chargement depuis une API source.</summary>
    Api,

    /// <summary>Rechargement manuel via l'API de management.</summary>
    Manual,

    /// <summary>Rechargement via le scheduler Quartz.</summary>
    Scheduled,

    /// <summary>Rechargement suite a un evenement RabbitMQ.</summary>
    EventDriven
}
