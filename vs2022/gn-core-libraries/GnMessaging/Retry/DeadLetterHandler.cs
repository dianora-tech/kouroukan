using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace GnMessaging.Retry;

/// <summary>
/// Gere les messages en echec permanent (apres exhaustion des retries).
/// Enregistre les details pour audit et diagnostic.
/// </summary>
public sealed class DeadLetterHandler
{
    private readonly ILogger<DeadLetterHandler> _logger;

    public DeadLetterHandler(ILogger<DeadLetterHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Appele quand un message echoue definitivement et est envoye en Dead Letter Queue.
    /// </summary>
    /// <param name="eventArgs">Arguments de l'evenement RabbitMQ.</param>
    /// <param name="exception">Exception ayant cause l'echec final.</param>
    public void OnMessageFailed(BasicDeliverEventArgs eventArgs, Exception exception)
    {
        var messageId = eventArgs.BasicProperties?.MessageId ?? "unknown";
        var correlationId = eventArgs.BasicProperties?.CorrelationId ?? "unknown";
        var routingKey = eventArgs.RoutingKey;
        var exchange = eventArgs.Exchange;

        string body;
        try
        {
            body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        }
        catch
        {
            body = "[impossible de decoder le body]";
        }

        _logger.LogError(exception,
            "Message envoye en DLQ — MessageId: {MessageId}, CorrelationId: {CorrelationId}, " +
            "Exchange: {Exchange}, RoutingKey: {RoutingKey}, Body: {Body}",
            messageId, correlationId, exchange, routingKey, TruncateBody(body));
    }

    private static string TruncateBody(string body, int maxLength = 1000)
    {
        return body.Length <= maxLength
            ? body
            : string.Concat(body.AsSpan(0, maxLength), "...[tronque]");
    }
}
