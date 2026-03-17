using GnCache.Application.Services;
using GnCache.Domain;
using GnCache.Infrastructure.Scheduling;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GnCache.Infrastructure.Services;

/// <summary>
/// Service de gestion du scheduler Quartz.NET pour les caches.
/// </summary>
public sealed class CacheSchedulerService : ICacheScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IReadOnlyList<CacheEntityRegistration> _registrations;
    private readonly ILogger<CacheSchedulerService> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CacheSchedulerService"/>.
    /// </summary>
    public CacheSchedulerService(
        ISchedulerFactory schedulerFactory,
        IReadOnlyList<CacheEntityRegistration> registrations,
        ILogger<CacheSchedulerService> logger)
    {
        ArgumentNullException.ThrowIfNull(schedulerFactory);
        ArgumentNullException.ThrowIfNull(registrations);
        ArgumentNullException.ThrowIfNull(logger);

        _schedulerFactory = schedulerFactory;
        _registrations = registrations;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CacheSchedule>> GetSchedulesAsync(
        CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var schedules = new List<CacheSchedule>();

        foreach (var registration in _registrations)
        {
            var jobKey = BuildJobKey(registration.CacheKey);
            var triggers = await scheduler.GetTriggersOfJob(jobKey, cancellationToken);
            var trigger = triggers.FirstOrDefault();

            schedules.Add(new CacheSchedule
            {
                CacheKey = registration.CacheKey,
                CronExpression = registration.CronExpression,
                SourceApiUrl = registration.SourceApiUrl,
                Enabled = trigger is not null,
                NextFireTimeUtc = trigger?.GetNextFireTimeUtc()?.UtcDateTime,
                LastFireTimeUtc = trigger?.GetPreviousFireTimeUtc()?.UtcDateTime
            });
        }

        return schedules.AsReadOnly();
    }

    /// <inheritdoc />
    public async Task UpdateScheduleAsync(string cacheKey, string newCronExpression,
        CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var triggerKey = BuildTriggerKey(cacheKey);

        var newTrigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .ForJob(BuildJobKey(cacheKey))
            .WithCronSchedule(newCronExpression)
            .Build();

        await scheduler.RescheduleJob(triggerKey, newTrigger, cancellationToken);

        _logger.LogInformation(
            "Schedule du cache '{CacheKey}' mis a jour avec le cron '{Cron}'.",
            cacheKey, newCronExpression);
    }

    /// <inheritdoc />
    public async Task TriggerAsync(string cacheKey,
        CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(BuildJobKey(cacheKey), cancellationToken);

        _logger.LogInformation(
            "Rechargement manuel du cache '{CacheKey}' declenche.",
            cacheKey);
    }

    private static JobKey BuildJobKey(string cacheKey)
        => new($"cache-refresh-{cacheKey}", "cache-jobs");

    private static TriggerKey BuildTriggerKey(string cacheKey)
        => new($"cache-trigger-{cacheKey}", "cache-triggers");
}
