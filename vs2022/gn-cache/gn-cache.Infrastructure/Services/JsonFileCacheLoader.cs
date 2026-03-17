using GnCache.Application.Services;
using GnCache.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GnCache.Infrastructure.Services;

/// <summary>
/// Service de chargement initial des caches depuis les fichiers JSON seed.
/// Execute au demarrage de l'application.
/// </summary>
public sealed class JsonFileCacheLoader
{
    private readonly IReadOnlyList<CacheEntityRegistration> _registrations;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JsonFileCacheLoader> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="JsonFileCacheLoader"/>.
    /// </summary>
    public JsonFileCacheLoader(
        IReadOnlyList<CacheEntityRegistration> registrations,
        IServiceProvider serviceProvider,
        ILogger<JsonFileCacheLoader> logger)
    {
        ArgumentNullException.ThrowIfNull(registrations);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(logger);

        _registrations = registrations;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Charge tous les caches depuis les fichiers JSON seed.
    /// </summary>
    public async Task LoadAllSeedsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Chargement des seeds JSON pour {Count} caches...",
            _registrations.Count);

        foreach (var registration in _registrations)
        {
            try
            {
                var serviceType = typeof(ICacheService<>).MakeGenericType(registration.EntityType);
                var service = _serviceProvider.GetService(serviceType);

                if (service is null)
                {
                    _logger.LogWarning(
                        "Service de cache introuvable pour '{CacheKey}'.",
                        registration.CacheKey);
                    continue;
                }

                var method = service.GetType().GetMethod("LoadFromSeedAsync");
                if (method is not null)
                {
                    var task = (Task?)method.Invoke(service, [cancellationToken]);
                    if (task is not null)
                        await task;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur lors du chargement seed du cache '{CacheKey}'.",
                    registration.CacheKey);
            }
        }

        _logger.LogInformation("Chargement des seeds termine.");
    }
}
