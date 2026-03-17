using GnCache.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GnCache.Api.Controllers;

/// <summary>
/// Controleur de gestion du scheduler de rechargement des caches.
/// </summary>
[ApiController]
[Route("api/scheduler")]
public sealed class CacheSchedulerController : ControllerBase
{
    private readonly ICacheScheduler _scheduler;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CacheSchedulerController"/>.
    /// </summary>
    public CacheSchedulerController(ICacheScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    /// <summary>
    /// Recupere la liste des jobs planifies.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSchedules(CancellationToken cancellationToken)
    {
        var schedules = await _scheduler.GetSchedulesAsync(cancellationToken);
        return Ok(schedules);
    }

    /// <summary>
    /// Modifie l'intervalle d'un job planifie.
    /// </summary>
    [HttpPut("{key}")]
    [Authorize(Policy = "RequirePermission:cache:reload")]
    public async Task<IActionResult> UpdateSchedule(
        string key,
        [FromBody] UpdateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        await _scheduler.UpdateScheduleAsync(key, request.CronExpression, cancellationToken);
        return Ok(new { message = $"Schedule du cache '{key}' mis a jour." });
    }

    /// <summary>
    /// Declenche manuellement un job de rechargement.
    /// </summary>
    [HttpPost("{key}/trigger")]
    [Authorize(Policy = "RequirePermission:cache:reload")]
    public async Task<IActionResult> TriggerJob(string key,
        CancellationToken cancellationToken)
    {
        await _scheduler.TriggerAsync(key, cancellationToken);
        return Accepted(new { message = $"Rechargement du cache '{key}' declenche." });
    }
}

/// <summary>
/// Requete de mise a jour du schedule.
/// </summary>
public sealed record UpdateScheduleRequest
{
    /// <summary>Nouvelle expression cron Quartz.</summary>
    public required string CronExpression { get; init; }
}
