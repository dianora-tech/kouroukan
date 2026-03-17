namespace GnMessaging.Models;

/// <summary>
/// Enveloppe standardisee pour tous les messages transitant par le bus.
/// Contient les metadonnees de routage, tracabilite et le payload serialise.
/// </summary>
public sealed class MessageEnvelope
{
    /// <summary>
    /// Identifiant unique du message.
    /// </summary>
    public Guid MessageId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifiant de correlation pour le suivi distribue.
    /// </summary>
    public Guid CorrelationId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Horodatage de creation du message (UTC).
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Service source du message.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Type complet du payload (pour la deserialisation).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Payload serialise en JSON.
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de tentatives de traitement effectuees.
    /// </summary>
    public int RetryCount { get; set; }
}
