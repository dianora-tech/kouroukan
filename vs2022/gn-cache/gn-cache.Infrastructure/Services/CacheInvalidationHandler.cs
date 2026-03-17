using GnCache.Application.Services;
using GnCache.Domain;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;

namespace GnCache.Infrastructure.Services;

/// <summary>
/// Handler de messages CacheInvalidatedEvent recus via RabbitMQ.
/// Recharge le cache concerne lorsqu'un autre microservice signale une modification.
/// </summary>
public sealed class CacheInvalidationHandler : IMessageHandler<CacheInvalidatedEvent>
{
    private readonly ICacheRegistry _registry;
    private readonly ILogger<CacheInvalidationHandler> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CacheInvalidationHandler"/>.
    /// </summary>
    public CacheInvalidationHandler(
        ICacheRegistry registry,
        ILogger<CacheInvalidationHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(logger);

        _registry = registry;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task HandleAsync(CacheInvalidatedEvent message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Evenement d'invalidation recu pour le cache '{CacheKey}'. Raison: {Reason}",
            message.CacheKey, message.Reason);

        var registration = _registry.GetRegistration(message.CacheKey);
        if (registration is null)
        {
            _logger.LogWarning(
                "Cache '{CacheKey}' introuvable dans le registre. Evenement ignore.",
                message.CacheKey);
            return;
        }

        await _registry.ReloadAsync(
            message.CacheKey,
            CacheSource.EventDriven,
            cancellationToken);

        _logger.LogInformation(
            "Cache '{CacheKey}' recharge suite a l'evenement d'invalidation.",
            message.CacheKey);
    }
}
