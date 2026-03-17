using GnCache.Domain;

namespace GnCache.Application.Services;

/// <summary>
/// Service de gestion du scheduler Quartz pour le rechargement periodique des caches.
/// </summary>
public interface ICacheScheduler
{
    /// <summary>
    /// Recupere la liste de tous les jobs planifies.
    /// </summary>
    Task<IReadOnlyList<CacheSchedule>> GetSchedulesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Modifie l'intervalle d'un job existant.
    /// </summary>
    /// <param name="cacheKey">Cle du cache.</param>
    /// <param name="newCronExpression">Nouvelle expression cron Quartz.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task UpdateScheduleAsync(string cacheKey, string newCronExpression,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Declenche manuellement un job de rechargement.
    /// </summary>
    Task TriggerAsync(string cacheKey, CancellationToken cancellationToken = default);
}
