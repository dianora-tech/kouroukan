using GnMessaging.Abstractions;

namespace GnMessaging.Events;

/// <summary>
/// Evenement emis lors de la creation d'une entite.
/// </summary>
/// <typeparam name="T">Type de l'entite creee.</typeparam>
public sealed class EntityCreatedEvent<T> : IMessage where T : class
{
    public EntityCreatedEvent()
    {
    }

    public EntityCreatedEvent(T data, int? userId = null)
    {
        Data = data;
        UserId = userId;
    }

    /// <inheritdoc />
    public Guid MessageId { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifiant de l'entite creee.
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Type de l'entite (nom complet du type .NET).
    /// </summary>
    public string EntityType { get; set; } = typeof(T).Name;

    /// <summary>
    /// Donnees completes de l'entite creee.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur ayant effectue la creation.
    /// </summary>
    public int? UserId { get; set; }
}
