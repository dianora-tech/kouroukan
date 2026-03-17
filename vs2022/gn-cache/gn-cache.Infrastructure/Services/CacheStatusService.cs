using System.Collections.Concurrent;
using GnCache.Application.Services;
using GnCache.Domain;

namespace GnCache.Infrastructure.Services;

/// <summary>
/// Service de monitoring du cache avec compteurs thread-safe.
/// </summary>
public sealed class CacheStatusService : ICacheStatusService
{
    private readonly ConcurrentDictionary<string, CacheStatistics> _stats = new();

    /// <inheritdoc />
    public Task<IReadOnlyList<CacheStatistics>> GetAllStatusAsync(
        CancellationToken cancellationToken = default)
    {
        var result = _stats.Values.ToList().AsReadOnly();
        return Task.FromResult<IReadOnlyList<CacheStatistics>>(result);
    }

    /// <inheritdoc />
    public Task<CacheStatistics?> GetStatusAsync(string cacheKey,
        CancellationToken cancellationToken = default)
    {
        _stats.TryGetValue(cacheKey, out var stats);
        return Task.FromResult(stats);
    }

    /// <inheritdoc />
    public void RecordL1Hit(string cacheKey)
    {
        var stats = GetOrCreate(cacheKey);
        Interlocked.Increment(ref stats.L1Hits);
    }

    /// <inheritdoc />
    public void RecordL2Hit(string cacheKey)
    {
        var stats = GetOrCreate(cacheKey);
        Interlocked.Increment(ref stats.L2Hits);
    }

    /// <inheritdoc />
    public void RecordMiss(string cacheKey)
    {
        var stats = GetOrCreate(cacheKey);
        Interlocked.Increment(ref stats.Misses);
    }

    /// <inheritdoc />
    public void RecordReload(string cacheKey, int itemCount, CacheSource source)
    {
        var stats = GetOrCreate(cacheKey);
        stats.ItemCount = itemCount;
        stats.LastRefreshedAtUtc = DateTime.UtcNow;
        stats.LastSource = source;
        stats.IsRefreshing = false;
        stats.LastError = null;
    }

    /// <inheritdoc />
    public void RecordError(string cacheKey, string error)
    {
        var stats = GetOrCreate(cacheKey);
        stats.LastError = error;
        stats.IsRefreshing = false;
    }

    private CacheStatistics GetOrCreate(string cacheKey)
    {
        return _stats.GetOrAdd(cacheKey, key => new CacheStatistics { CacheKey = key });
    }
}
