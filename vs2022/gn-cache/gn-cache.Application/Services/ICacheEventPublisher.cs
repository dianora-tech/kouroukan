namespace GnCache.Application.Services;

/// <summary>
/// Service de publication d'evenements de cache sur RabbitMQ.
/// </summary>
public interface ICacheEventPublisher
{
    /// <summary>
    /// Publie un evenement d'invalidation de cache sur le bus.
    /// </summary>
    /// <param name="cacheKey">Cle du cache invalide.</param>
    /// <param name="reason">Raison de l'invalidation.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task PublishInvalidationAsync(string cacheKey, string? reason = null,
        CancellationToken cancellationToken = default);
}
