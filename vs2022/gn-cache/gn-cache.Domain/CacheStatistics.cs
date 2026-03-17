namespace GnCache.Domain;

/// <summary>
/// Statistiques d'utilisation d'un cache.
/// </summary>
public sealed class CacheStatistics
{
    /// <summary>Cle du cache.</summary>
    public string CacheKey { get; init; } = string.Empty;

    /// <summary>Nombre de hits L1 (MemoryCache). Champ pour Interlocked.Increment.</summary>
    public long L1Hits;

    /// <summary>Nombre de hits L2 (Redis). Champ pour Interlocked.Increment.</summary>
    public long L2Hits;

    /// <summary>Nombre de miss total. Champ pour Interlocked.Increment.</summary>
    public long Misses;

    /// <summary>Nombre d'elements actuellement en cache.</summary>
    public int ItemCount { get; set; }

    /// <summary>Date du dernier rechargement reussi (UTC).</summary>
    public DateTime? LastRefreshedAtUtc { get; set; }

    /// <summary>Source du dernier rechargement.</summary>
    public CacheSource? LastSource { get; set; }

    /// <summary>Si le cache est actuellement en cours de rechargement.</summary>
    public bool IsRefreshing { get; set; }

    /// <summary>Message d'erreur du dernier echec de rechargement.</summary>
    public string? LastError { get; set; }

    /// <summary>Taux de hit global (L1 + L2) en pourcentage.</summary>
    public double HitRatePercent
    {
        get
        {
            var total = L1Hits + L2Hits + Misses;
            return total == 0 ? 0 : (double)(L1Hits + L2Hits) / total * 100;
        }
    }
}
