using System.Text.Json;
using GnCache.Application.Services;
using GnCache.Domain;
using GnCache.Infrastructure.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GnCache.Infrastructure.Services;

/// <summary>
/// Implementation generique du cache a deux niveaux (L1 MemoryCache + L2 Redis).
/// Gere le chargement initial, le rechargement et le fallback si Redis est indisponible.
/// </summary>
/// <typeparam name="T">Type de l'entite mise en cache.</typeparam>
public class BaseCacheService<T> : ICacheService<T> where T : class
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ICacheStatusService _statusService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CacheOptions _options;
    private readonly CacheEntityRegistration _registration;
    private readonly ILogger<BaseCacheService<T>> _logger;
    private readonly SemaphoreSlim _reloadLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="BaseCacheService{T}"/>.
    /// </summary>
    public BaseCacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ICacheStatusService statusService,
        IHttpClientFactory httpClientFactory,
        IOptions<CacheOptions> options,
        CacheEntityRegistration registration,
        ILogger<BaseCacheService<T>> logger)
    {
        ArgumentNullException.ThrowIfNull(memoryCache);
        ArgumentNullException.ThrowIfNull(distributedCache);
        ArgumentNullException.ThrowIfNull(statusService);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(registration);
        ArgumentNullException.ThrowIfNull(logger);

        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _statusService = statusService;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _registration = registration;
        _logger = logger;
    }

    /// <inheritdoc />
    public string CacheKey => _registration.CacheKey;

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var l1Key = BuildCacheKey();

        // 1. Try L1 (MemoryCache)
        if (_memoryCache.TryGetValue(l1Key, out IReadOnlyList<T>? cached) && cached is not null)
        {
            _statusService.RecordL1Hit(_registration.CacheKey);
            return cached;
        }

        // 2. Try L2 (Redis) with fallback on error
        try
        {
            var redisData = await _distributedCache.GetStringAsync(l1Key, cancellationToken);
            if (redisData is not null)
            {
                var data = JsonSerializer.Deserialize<List<T>>(redisData, JsonOptions) ?? [];
                var readOnly = data.AsReadOnly();

                // Promote to L1
                _memoryCache.Set(l1Key, readOnly, _registration.MemoryTtl);
                _statusService.RecordL2Hit(_registration.CacheKey);
                return readOnly;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Redis indisponible pour le cache '{CacheKey}'. Fallback L1.",
                _registration.CacheKey);
        }

        // 3. Miss -- reload from source
        _statusService.RecordMiss(_registration.CacheKey);
        await ReloadAsync(CacheSource.Api, cancellationToken);

        // Try L1 again after reload
        if (_memoryCache.TryGetValue(l1Key, out cached) && cached is not null)
            return cached;

        return [];
    }

    /// <inheritdoc />
    public async Task ReloadAsync(CacheSource source = CacheSource.Manual,
        CancellationToken cancellationToken = default)
    {
        if (!await _reloadLock.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken))
        {
            _logger.LogWarning(
                "Rechargement du cache '{CacheKey}' deja en cours, ignore.",
                _registration.CacheKey);
            return;
        }

        try
        {
            _logger.LogInformation(
                "Rechargement du cache '{CacheKey}' depuis {Source}...",
                _registration.CacheKey, source);

            var client = _httpClientFactory.CreateClient("CacheApiClient");
            var response = await client.GetAsync(_registration.SourceApiUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? [];
            var readOnly = data.AsReadOnly();

            await StoreInCacheAsync(readOnly, cancellationToken);
            _statusService.RecordReload(_registration.CacheKey, readOnly.Count, source);

            _logger.LogInformation(
                "Cache '{CacheKey}' recharge avec succes. {Count} elements.",
                _registration.CacheKey, readOnly.Count);
        }
        catch (Exception ex)
        {
            _statusService.RecordError(_registration.CacheKey, ex.Message);
            _logger.LogError(ex,
                "Erreur lors du rechargement du cache '{CacheKey}'.",
                _registration.CacheKey);
        }
        finally
        {
            _reloadLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task LoadFromSeedAsync(CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_options.SeedDataPath, _registration.SeedFileName);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning(
                "Fichier seed introuvable pour le cache '{CacheKey}': {Path}",
                _registration.CacheKey, filePath);
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            var data = JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? [];
            var readOnly = data.AsReadOnly();

            await StoreInCacheAsync(readOnly, cancellationToken);
            _statusService.RecordReload(_registration.CacheKey, readOnly.Count, CacheSource.JsonSeed);

            _logger.LogInformation(
                "Cache '{CacheKey}' charge depuis le seed. {Count} elements.",
                _registration.CacheKey, readOnly.Count);
        }
        catch (Exception ex)
        {
            _statusService.RecordError(_registration.CacheKey, ex.Message);
            _logger.LogError(ex,
                "Erreur lors du chargement seed du cache '{CacheKey}'.",
                _registration.CacheKey);
        }
    }

    /// <inheritdoc />
    public async Task InvalidateAsync(CancellationToken cancellationToken = default)
    {
        var l1Key = BuildCacheKey();
        _memoryCache.Remove(l1Key);

        try
        {
            await _distributedCache.RemoveAsync(l1Key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Impossible de supprimer le cache Redis '{CacheKey}'.",
                _registration.CacheKey);
        }

        _logger.LogInformation("Cache '{CacheKey}' invalide.", _registration.CacheKey);
    }

    private async Task StoreInCacheAsync(IReadOnlyList<T> data,
        CancellationToken cancellationToken)
    {
        var l1Key = BuildCacheKey();

        // Store in L1
        _memoryCache.Set(l1Key, data, _registration.MemoryTtl);

        // Store in L2 (Redis) -- non-fatal on error
        try
        {
            var json = JsonSerializer.Serialize(data, JsonOptions);
            await _distributedCache.SetStringAsync(
                l1Key,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _registration.RedisTtl
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Impossible d'ecrire dans Redis pour le cache '{CacheKey}'. L1 seul actif.",
                _registration.CacheKey);
        }
    }

    private string BuildCacheKey() => $"{_options.RedisKeyPrefix}{_registration.CacheKey}";
}
