using GnCache.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GnCache.Infrastructure.Startup;

/// <summary>
/// Service d'initialisation au demarrage de l'application.
/// Charge les seeds JSON puis recharge depuis les API.
/// </summary>
public sealed class CacheStartupService : IHostedService
{
    private readonly JsonFileCacheLoader _seedLoader;
    private readonly DatabaseCacheLoader _apiLoader;
    private readonly ILogger<CacheStartupService> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CacheStartupService"/>.
    /// </summary>
    public CacheStartupService(
        JsonFileCacheLoader seedLoader,
        DatabaseCacheLoader apiLoader,
        ILogger<CacheStartupService> logger)
    {
        ArgumentNullException.ThrowIfNull(seedLoader);
        ArgumentNullException.ThrowIfNull(apiLoader);
        ArgumentNullException.ThrowIfNull(logger);

        _seedLoader = seedLoader;
        _apiLoader = apiLoader;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initialisation du systeme de cache...");

        // Phase 1: Charger les seeds (rapide, pas de dependance externe)
        await _seedLoader.LoadAllSeedsAsync(cancellationToken);

        // Phase 2: Recharger depuis les API (peut echouer, les seeds sont le fallback)
        try
        {
            await _apiLoader.ReloadAllFromApiAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Rechargement API echoue au demarrage. Les donnees seed sont conservees.");
        }

        _logger.LogInformation("Systeme de cache initialise.");
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
