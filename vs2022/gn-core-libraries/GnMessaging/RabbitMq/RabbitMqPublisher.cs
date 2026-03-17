using System.Text;
using System.Text.Json;
using GnMessaging.Abstractions;
using GnMessaging.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace GnMessaging.RabbitMq;

/// <summary>
/// Implementation RabbitMQ de IMessagePublisher.
/// Serialise le message, l'enveloppe dans un MessageEnvelope et le publie sur l'exchange.
/// </summary>
public sealed class RabbitMqPublisher : IMessagePublisher
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(
        RabbitMqConnectionManager connectionManager,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqPublisher> logger)
    {
        _connectionManager = connectionManager;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task PublishAsync<T>(T message, string exchange, string routingKey,
        PublishOptions? options = null, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(exchange);
        ArgumentException.ThrowIfNullOrWhiteSpace(routingKey);

        cancellationToken.ThrowIfCancellationRequested();

        var envelope = new MessageEnvelope
        {
            MessageId = message.MessageId,
            CorrelationId = options?.CorrelationId ?? Guid.NewGuid(),
            Timestamp = message.Timestamp,
            Source = _options.ServiceName,
            Type = typeof(T).AssemblyQualifiedName ?? typeof(T).FullName ?? typeof(T).Name,
            Payload = JsonSerializer.Serialize(message)
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope));

        using var channel = _connectionManager.CreateChannel();

        // Declare l'exchange (idempotent)
        channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true, autoDelete: false);

        var properties = channel.CreateBasicProperties();
        properties.MessageId = envelope.MessageId.ToString();
        properties.CorrelationId = envelope.CorrelationId.ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.ContentType = "application/json";
        properties.ContentEncoding = "utf-8";
        properties.Type = typeof(T).Name;
        properties.DeliveryMode = (options?.Persistent ?? true) ? (byte)2 : (byte)1;
        properties.Priority = options?.Priority ?? 0;
        properties.Headers = new Dictionary<string, object>
        {
            ["x-source"] = _options.ServiceName,
            ["x-message-type"] = typeof(T).FullName ?? typeof(T).Name
        };

        channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        _logger.LogDebug(
            "Message {MessageType} publie sur {Exchange}/{RoutingKey} (Id: {MessageId})",
            typeof(T).Name, exchange, routingKey, envelope.MessageId);

        return Task.CompletedTask;
    }
}
