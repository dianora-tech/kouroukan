using GnMessaging.Abstractions;

namespace GnMessaging.Events;

/// <summary>
/// Evenement emis lors de la suppression d'une entite.
/// </summary>
/// <typeparam name="T">Type de l'entite supprimee.</typeparam>
public sealed class EntityDeletedEvent<T> : IMessage where T : class
{
    public EntityDeletedEvent()
    {
    }

    public EntityDeletedEvent(int entityId, int? userId = null)
    {
        EntityId = entityId;
        UserId = userId;
    }

    /// <inheritdoc />
    public Guid MessageId { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifiant de l'entite supprimee.
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Type de l'entite.
    /// </summary>
    public string EntityType { get; set; } = typeof(T).Name;

    /// <summary>
    /// Identifiant de l'utilisateur ayant effectue la suppression.
    /// </summary>
    public int? UserId { get; set; }
}
