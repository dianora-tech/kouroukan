namespace GnCache.Domain;

/// <summary>
/// Represente la configuration de planification d'un cache.
/// </summary>
public sealed class CacheSchedule
{
    /// <summary>Cle du cache (ex: "regions").</summary>
    public string CacheKey { get; init; } = string.Empty;

    /// <summary>Expression cron Quartz (6 champs avec secondes).</summary>
    public string CronExpression { get; init; } = string.Empty;

    /// <summary>URL de l'API source pour le rechargement.</summary>
    public string SourceApiUrl { get; init; } = string.Empty;

    /// <summary>Si true, le job est actif.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Prochaine execution prevue (UTC).</summary>
    public DateTime? NextFireTimeUtc { get; set; }

    /// <summary>Derniere execution (UTC).</summary>
    public DateTime? LastFireTimeUtc { get; set; }
}
