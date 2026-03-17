using System.Text;
using System.Text.Json;
using GnMessaging.Abstractions;
using GnMessaging.Models;
using GnMessaging.Retry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GnMessaging.RabbitMq;

/// <summary>
/// Implementation RabbitMQ de IMessageConsumer.
/// BackgroundService qui ecoute les queues et route les messages vers les handlers enregistres.
/// </summary>
public sealed class RabbitMqConsumer : BackgroundService, IMessageConsumer
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly RabbitMqOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RetryPolicy _retryPolicy;
    private readonly DeadLetterHandler _deadLetterHandler;
    private readonly List<ConsumerRegistration> _registrations = [];
    private IModel? _channel;

    public RabbitMqConsumer(
        RabbitMqConnectionManager connectionManager,
        IOptions<RabbitMqOptions> options,
        IServiceProvider serviceProvider,
        ILogger<RabbitMqConsumer> logger,
        RetryPolicy retryPolicy,
        DeadLetterHandler deadLetterHandler)
    {
        _connectionManager = connectionManager;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _retryPolicy = retryPolicy;
        _deadLetterHandler = deadLetterHandler;
    }

    /// <inheritdoc />
    public Task StartConsumingAsync<T>(ConsumeOptions options, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        _registrations.Add(new ConsumerRegistration
        {
            MessageType = typeof(T),
            Options = options
        });

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Attendre un court instant pour que les registrations soient faites
        await Task.Delay(500, stoppingToken);

        if (_registrations.Count == 0)
        {
            _logger.LogInformation("Aucun consumer enregistre. Le BackgroundService reste en veille.");
            return;
        }

        _channel = _connectionManager.CreateChannel();

        foreach (var registration in _registrations)
        {
            SetupConsumer(registration, stoppingToken);
        }

        _logger.LogInformation(
            "{Count} consumer(s) RabbitMQ demarres pour le service {Service}.",
            _registrations.Count, _options.ServiceName);

        // Reste actif tant que le service tourne
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Arret normal
        }
    }

    private void SetupConsumer(ConsumerRegistration registration, CancellationToken stoppingToken)
    {
        var options = registration.Options;

        // Declare l'exchange si specifie
        if (!string.IsNullOrWhiteSpace(options.Exchange))
        {
            _channel!.ExchangeDeclare(options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
        }

        // Dead Letter Exchange
        var dlxExchange = $"{options.QueueName}.dlx";
        var dlqQueue = $"{options.QueueName}.dlq";

        _channel!.ExchangeDeclare(dlxExchange, ExchangeType.Fanout, durable: true, autoDelete: false);
        _channel!.QueueDeclare(dlqQueue, durable: true, exclusive: false, autoDelete: false);
        _channel!.QueueBind(dlqQueue, dlxExchange, string.Empty);

        // Declare la queue avec DLX
        var queueArgs = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = dlxExchange
        };

        _channel!.QueueDeclare(
            queue: options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs);

        // Bind aux routing keys
        if (!string.IsNullOrWhiteSpace(options.Exchange))
        {
            foreach (var bindingKey in options.BindingKeys)
            {
                _channel.QueueBind(options.QueueName, options.Exchange, bindingKey);
            }
        }

        _channel.BasicQos(0, options.PrefetchCount, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            await HandleMessageAsync(ea, registration, stoppingToken);
        };

        _channel.BasicConsume(
            queue: options.QueueName,
            autoAck: options.AutoAck,
            consumer: consumer);

        _logger.LogInformation(
            "Consumer demarre sur queue {Queue} pour le type {MessageType}",
            options.QueueName, registration.MessageType.Name);
    }

    private async Task HandleMessageAsync(
        BasicDeliverEventArgs ea,
        ConsumerRegistration registration,
        CancellationToken stoppingToken)
    {
        var messageType = registration.MessageType;
        var maxRetries = registration.Options.RetryCount;

        try
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            var envelope = JsonSerializer.Deserialize<MessageEnvelope>(body);

            if (envelope is null)
            {
                _logger.LogWarning("Message recu avec enveloppe null. Rejected.");
                _channel!.BasicReject(ea.DeliveryTag, requeue: false);
                return;
            }

            var message = JsonSerializer.Deserialize(envelope.Payload, messageType);
            if (message is null)
            {
                _logger.LogWarning(
                    "Impossible de deserialiser le payload en {Type}. Rejected.",
                    messageType.Name);
                _channel!.BasicReject(ea.DeliveryTag, requeue: false);
                return;
            }

            await _retryPolicy.ExecuteAsync(async () =>
            {
                using var scope = _serviceProvider.CreateScope();

                var handlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);
                var handler = scope.ServiceProvider.GetService(handlerType);

                if (handler is null)
                {
                    _logger.LogWarning(
                        "Aucun handler enregistre pour le type {MessageType}. Message ignore.",
                        messageType.Name);
                    return;
                }

                var method = handlerType.GetMethod("HandleAsync")!;
                var task = (Task)method.Invoke(handler, [message, stoppingToken])!;
                await task;
            }, maxRetries, stoppingToken);

            if (!registration.Options.AutoAck)
            {
                _channel!.BasicAck(ea.DeliveryTag, multiple: false);
            }

            _logger.LogDebug(
                "Message {MessageId} traite avec succes (type: {Type})",
                envelope.MessageId, messageType.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Echec du traitement du message (type: {Type}). Envoi en DLQ.",
                messageType.Name);

            if (!registration.Options.AutoAck)
            {
                // Reject sans requeue → va en DLX/DLQ
                _channel!.BasicReject(ea.DeliveryTag, requeue: false);
            }

            _deadLetterHandler.OnMessageFailed(ea, ex);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }

    private sealed class ConsumerRegistration
    {
        public Type MessageType { get; init; } = null!;
        public ConsumeOptions Options { get; init; } = null!;
    }
}
