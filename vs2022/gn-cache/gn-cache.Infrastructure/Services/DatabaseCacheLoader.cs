using GnCache.Application.Services;
using GnCache.Domain;
using Microsoft.Extensions.Logging;

namespace GnCache.Infrastructure.Services;

/// <summary>
/// Service de rechargement des caches depuis les API sources.
/// Execute au demarrage apres le JsonFileCacheLoader pour obtenir des donnees fraiches.
/// </summary>
public sealed class DatabaseCacheLoader
{
    private readonly ICacheRegistry _registry;
    private readonly ILogger<DatabaseCacheLoader> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="DatabaseCacheLoader"/>.
    /// </summary>
    public DatabaseCacheLoader(
        ICacheRegistry registry,
        ILogger<DatabaseCacheLoader> logger)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(logger);

        _registry = registry;
        _logger = logger;
    }

    /// <summary>
    /// Recharge tous les caches depuis les API sources.
    /// Si une API est indisponible, les donnees seed sont conservees.
    /// </summary>
    public async Task ReloadAllFromApiAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Rechargement des caches depuis les API...");

        foreach (var registration in _registry.GetAllRegistrations())
        {
            try
            {
                await _registry.ReloadAsync(
                    registration.CacheKey,
                    CacheSource.Api,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Rechargement API echoue pour le cache '{CacheKey}'. Donnees seed conservees.",
                    registration.CacheKey);
            }
        }

        _logger.LogInformation("Rechargement API termine.");
    }
}
