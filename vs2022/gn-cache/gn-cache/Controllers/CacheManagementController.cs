using GnCache.Application.Services;
using GnCache.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GnCache.Api.Controllers;

/// <summary>
/// Controleur de gestion des caches. Lecture et rechargement.
/// </summary>
[ApiController]
[Route("api/cache")]
public sealed class CacheManagementController : ControllerBase
{
    private readonly ICacheRegistry _registry;
    private readonly ICacheStatusService _statusService;
    private readonly ICacheEventPublisher _eventPublisher;
    private readonly ILogger<CacheManagementController> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CacheManagementController"/>.
    /// </summary>
    public CacheManagementController(
        ICacheRegistry registry,
        ICacheStatusService statusService,
        ICacheEventPublisher eventPublisher,
        ILogger<CacheManagementController> logger)
    {
        _registry = registry;
        _statusService = statusService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Recupere le statut de tous les caches.
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
    {
        var statuses = await _statusService.GetAllStatusAsync(cancellationToken);
        return Ok(new { caches = statuses });
    }

    /// <summary>
    /// Recupere les donnees d'un cache par sa cle.
    /// </summary>
    [HttpGet("{key}")]
    public async Task<IActionResult> GetCache(string key,
        CancellationToken cancellationToken)
    {
        var registration = _registry.GetRegistration(key);
        if (registration is null)
            return NotFound(new { message = $"Cache '{key}' introuvable." });

        var data = await _registry.GetDataAsync(key, cancellationToken);
        return Ok(data);
    }

    /// <summary>
    /// Recharge un cache specifique.
    /// </summary>
    [HttpPost("{key}/reload")]
    [Authorize(Policy = "RequirePermission:cache:reload")]
    public async Task<IActionResult> ReloadCache(string key,
        CancellationToken cancellationToken)
    {
        var registration = _registry.GetRegistration(key);
        if (registration is null)
            return NotFound(new { message = $"Cache '{key}' introuvable." });

        await _registry.ReloadAsync(key, CacheSource.Manual, cancellationToken);
        await _eventPublisher.PublishInvalidationAsync(key, "Manual reload via API",
            cancellationToken);

        _logger.LogInformation("Cache '{Key}' recharge manuellement.", key);
        return Ok(new { message = $"Cache '{key}' recharge avec succes." });
    }

    /// <summary>
    /// Recharge tous les caches.
    /// </summary>
    [HttpPost("reload-all")]
    [Authorize(Policy = "RequirePermission:cache:reload")]
    public async Task<IActionResult> ReloadAll(CancellationToken cancellationToken)
    {
        await _registry.ReloadAllAsync(CacheSource.Manual, cancellationToken);

        foreach (var reg in _registry.GetAllRegistrations())
        {
            await _eventPublisher.PublishInvalidationAsync(
                reg.CacheKey, "Manual reload-all via API", cancellationToken);
        }

        _logger.LogInformation("Tous les caches recharges manuellement.");
        return Ok(new { message = "Tous les caches recharges avec succes." });
    }
}
