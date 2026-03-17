using GnMessaging.Abstractions;

namespace GnMessaging.Events;

/// <summary>
/// Evenement emis lors de la modification d'une entite.
/// </summary>
/// <typeparam name="T">Type de l'entite modifiee.</typeparam>
public sealed class EntityUpdatedEvent<T> : IMessage where T : class
{
    public EntityUpdatedEvent()
    {
    }

    public EntityUpdatedEvent(T data, int? userId = null)
    {
        Data = data;
        UserId = userId;
    }

    /// <inheritdoc />
    public Guid MessageId { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifiant de l'entite modifiee.
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Type de l'entite.
    /// </summary>
    public string EntityType { get; set; } = typeof(T).Name;

    /// <summary>
    /// Donnees actualisees de l'entite.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur ayant effectue la modification.
    /// </summary>
    public int? UserId { get; set; }
}
