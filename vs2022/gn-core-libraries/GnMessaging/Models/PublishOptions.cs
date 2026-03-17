namespace GnMessaging.Models;

/// <summary>
/// Options de publication d'un message sur RabbitMQ.
/// </summary>
public sealed class PublishOptions
{
    /// <summary>
    /// Nom de l'exchange cible. Si null, utilise l'exchange par defaut de la configuration.
    /// </summary>
    public string? Exchange { get; set; }

    /// <summary>
    /// Cle de routage. Si null, utilise le type du message comme routing key.
    /// </summary>
    public string? RoutingKey { get; set; }

    /// <summary>
    /// Si true, le message est persistant (survit au restart de RabbitMQ). Defaut: true.
    /// </summary>
    public bool Persistent { get; set; } = true;

    /// <summary>
    /// Priorite du message (0-9, 0 = la plus basse). Defaut: 0.
    /// </summary>
    public byte Priority { get; set; }

    /// <summary>
    /// CorrelationId optionnel pour le suivi distribue.
    /// Si null, un nouveau CorrelationId est genere automatiquement.
    /// </summary>
    public Guid? CorrelationId { get; set; }
}
