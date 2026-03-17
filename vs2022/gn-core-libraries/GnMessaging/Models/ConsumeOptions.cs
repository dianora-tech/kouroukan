namespace GnMessaging.Models;

/// <summary>
/// Options de consommation de messages depuis RabbitMQ.
/// </summary>
public sealed class ConsumeOptions
{
    /// <summary>
    /// Nom de la queue a consommer.
    /// </summary>
    public string QueueName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de messages prefetch (non traites en parallele). Defaut: 10.
    /// </summary>
    public ushort PrefetchCount { get; set; } = 10;

    /// <summary>
    /// Si true, les messages sont acquittes automatiquement. Defaut: false (ack manuel recommande).
    /// </summary>
    public bool AutoAck { get; set; }

    /// <summary>
    /// Nombre maximum de tentatives avant envoi en Dead Letter Queue. Defaut: 5.
    /// </summary>
    public int RetryCount { get; set; } = 5;

    /// <summary>
    /// Exchange auquel binder la queue (type topic).
    /// </summary>
    public string? Exchange { get; set; }

    /// <summary>
    /// Routing keys a binder sur la queue (patterns avec wildcards '#' et '*').
    /// </summary>
    public string[] BindingKeys { get; set; } = ["#"];
}
