using GnMessaging.Abstractions;

namespace GnMessaging.Events;

/// <summary>
/// Evenement emis pour invalider une entree dans le cache distribue.
/// Le service gn-cache ecoute ces evenements pour mettre a jour L1/L2.
/// </summary>
public sealed class CacheInvalidatedEvent : IMessage
{
    public CacheInvalidatedEvent()
    {
    }

    public CacheInvalidatedEvent(string cacheKey, string? reason = null)
    {
        CacheKey = cacheKey;
        Reason = reason;
    }

    /// <inheritdoc />
    public Guid MessageId { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Cle du cache a invalider.
    /// </summary>
    public string CacheKey { get; set; } = string.Empty;

    /// <summary>
    /// Raison de l'invalidation (optionnel, pour audit).
    /// </summary>
    public string? Reason { get; set; }
}
