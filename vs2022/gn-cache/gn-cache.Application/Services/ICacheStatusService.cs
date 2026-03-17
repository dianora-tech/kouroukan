using GnCache.Domain;

namespace GnCache.Application.Services;

/// <summary>
/// Service de monitoring et statistiques du cache.
/// </summary>
public interface ICacheStatusService
{
    /// <summary>
    /// Recupere le statut de tous les caches enregistres.
    /// </summary>
    Task<IReadOnlyList<CacheStatistics>> GetAllStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupere le statut d'un cache specifique.
    /// </summary>
    Task<CacheStatistics?> GetStatusAsync(string cacheKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Incremente le compteur de hit L1 pour un cache.
    /// </summary>
    void RecordL1Hit(string cacheKey);

    /// <summary>
    /// Incremente le compteur de hit L2 pour un cache.
    /// </summary>
    void RecordL2Hit(string cacheKey);

    /// <summary>
    /// Incremente le compteur de miss pour un cache.
    /// </summary>
    void RecordMiss(string cacheKey);

    /// <summary>
    /// Met a jour le nombre d'elements et les metadonnees apres un rechargement.
    /// </summary>
    void RecordReload(string cacheKey, int itemCount, CacheSource source);

    /// <summary>
    /// Enregistre une erreur de rechargement.
    /// </summary>
    void RecordError(string cacheKey, string error);
}
