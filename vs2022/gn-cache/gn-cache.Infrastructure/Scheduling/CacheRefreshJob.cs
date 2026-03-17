using GnCache.Application.Services;
using GnCache.Domain;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GnCache.Infrastructure.Scheduling;

/// <summary>
/// Job Quartz.NET generique pour le rechargement periodique d'un cache.
/// Le CacheKey est passe via le JobDataMap.
/// </summary>
[DisallowConcurrentExecution]
public sealed class CacheRefreshJob : IJob
{
    /// <summary>Cle du JobDataMap pour la cle de cache.</summary>
    public const string CacheKeyDataKey = "CacheKey";

    private readonly ICacheRegistry _registry;
    private readonly ICacheEventPublisher _eventPublisher;
    private readonly ILogger<CacheRefreshJob> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CacheRefreshJob"/>.
    /// </summary>
    public CacheRefreshJob(
        ICacheRegistry registry,
        ICacheEventPublisher eventPublisher,
        ILogger<CacheRefreshJob> logger)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(eventPublisher);
        ArgumentNullException.ThrowIfNull(logger);

        _registry = registry;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        var cacheKey = context.MergedJobDataMap.GetString(CacheKeyDataKey);

        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            _logger.LogError("CacheRefreshJob execute sans CacheKey dans le JobDataMap.");
            return;
        }

        _logger.LogInformation("Rechargement planifie du cache '{CacheKey}'...", cacheKey);

        try
        {
            await _registry.ReloadAsync(cacheKey, CacheSource.Scheduled);

            await _eventPublisher.PublishInvalidationAsync(
                cacheKey, "Scheduled refresh");

            _logger.LogInformation("Cache '{CacheKey}' recharge avec succes.", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erreur lors du rechargement planifie du cache '{CacheKey}'.", cacheKey);
            throw new JobExecutionException(ex, refireImmediately: false);
        }
    }
}
