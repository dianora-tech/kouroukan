using GnCache.Application.Services;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.RabbitMq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GnCache.Infrastructure.Services;

/// <summary>
/// Publication d'evenements d'invalidation de cache sur RabbitMQ.
/// Utilise le IMessagePublisher de GnMessaging.
/// </summary>
public sealed class CacheEventPublisher : ICacheEventPublisher
{
    private readonly IMessagePublisher _publisher;
    private readonly RabbitMqOptions _rabbitOptions;
    private readonly ILogger<CacheEventPublisher> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CacheEventPublisher"/>.
    /// </summary>
    public CacheEventPublisher(
        IMessagePublisher publisher,
        IOptions<RabbitMqOptions> rabbitOptions,
        ILogger<CacheEventPublisher> logger)
    {
        ArgumentNullException.ThrowIfNull(publisher);
        ArgumentNullException.ThrowIfNull(rabbitOptions);
        ArgumentNullException.ThrowIfNull(logger);

        _publisher = publisher;
        _rabbitOptions = rabbitOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishInvalidationAsync(string cacheKey, string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var @event = new CacheInvalidatedEvent(cacheKey, reason);

        try
        {
            await _publisher.PublishAsync(
                @event,
                _rabbitOptions.DefaultExchange,
                $"cache.invalidated.{cacheKey}",
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Evenement d'invalidation publie pour le cache '{CacheKey}'.",
                cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erreur lors de la publication de l'evenement d'invalidation pour '{CacheKey}'.",
                cacheKey);
        }
    }
}
