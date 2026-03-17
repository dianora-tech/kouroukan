using GnCache.Application.Services;
using GnCache.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GnCache.Infrastructure.Services;

/// <summary>
/// Registre central de tous les caches declares.
/// Resout dynamiquement les ICacheService{T} via le conteneur DI.
/// </summary>
public sealed class CacheRegistry : ICacheRegistry
{
    private readonly IReadOnlyList<CacheEntityRegistration> _registrations;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CacheRegistry> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CacheRegistry"/>.
    /// </summary>
    public CacheRegistry(
        IReadOnlyList<CacheEntityRegistration> registrations,
        IServiceProvider serviceProvider,
        ILogger<CacheRegistry> logger)
    {
        ArgumentNullException.ThrowIfNull(registrations);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(logger);

        _registrations = registrations;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public IReadOnlyList<CacheEntityRegistration> GetAllRegistrations() => _registrations;

    /// <inheritdoc />
    public CacheEntityRegistration? GetRegistration(string cacheKey)
        => _registrations.FirstOrDefault(r =>
            r.CacheKey.Equals(cacheKey, StringComparison.OrdinalIgnoreCase));

    /// <inheritdoc />
    public async Task ReloadAllAsync(CacheSource source, CancellationToken cancellationToken)
    {
        var tasks = _registrations.Select(reg =>
            ReloadAsync(reg.CacheKey, source, cancellationToken));
        await Task.WhenAll(tasks);
    }

    /// <inheritdoc />
    public async Task ReloadAsync(string cacheKey, CacheSource source,
        CancellationToken cancellationToken)
    {
        var service = ResolveCacheServiceInternal(cacheKey);
        if (service is null)
        {
            _logger.LogWarning("Cache '{CacheKey}' introuvable dans le registre.", cacheKey);
            return;
        }

        // Invoke ReloadAsync via dynamic dispatch on the non-generic interface method
        var method = service.GetType().GetMethod("ReloadAsync");
        if (method is not null)
        {
            var task = (Task?)method.Invoke(service, [source, cancellationToken]);
            if (task is not null)
                await task;
        }
    }

    /// <inheritdoc />
    public async Task<object?> GetDataAsync(string cacheKey,
        CancellationToken cancellationToken)
    {
        var service = ResolveCacheServiceInternal(cacheKey);
        if (service is null)
            return null;

        var method = service.GetType().GetMethod("GetAllAsync");
        if (method is null)
            return null;

        var task = method.Invoke(service, [cancellationToken]);
        if (task is null)
            return null;

        await (Task)task;

        // Get the result from the Task<IReadOnlyList<T>>
        var resultProperty = task.GetType().GetProperty("Result");
        return resultProperty?.GetValue(task);
    }

    private object? ResolveCacheServiceInternal(string cacheKey)
    {
        var registration = GetRegistration(cacheKey);
        if (registration is null)
            return null;

        var serviceType = typeof(ICacheService<>).MakeGenericType(registration.EntityType);
        return _serviceProvider.GetService(serviceType);
    }
}
