namespace GnMessaging.Abstractions;

/// <summary>
/// Interface marqueur pour identifier les messages transitant par le bus.
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Identifiant unique du message.
    /// </summary>
    Guid MessageId { get; }

    /// <summary>
    /// Horodatage de creation du message (UTC).
    /// </summary>
    DateTime Timestamp { get; }
}
