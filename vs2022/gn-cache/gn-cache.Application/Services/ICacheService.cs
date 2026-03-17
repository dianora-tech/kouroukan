using GnCache.Domain;

namespace GnCache.Application.Services;

/// <summary>
/// Service generique de gestion du cache pour un type d'entite.
/// </summary>
/// <typeparam name="T">Type de l'entite mise en cache.</typeparam>
public interface ICacheService<T> where T : class
{
    /// <summary>
    /// Recupere toutes les entites depuis le cache (L1 puis L2 puis source).
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Recharge le cache depuis l'API source.
    /// </summary>
    /// <param name="source">Source du rechargement.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task ReloadAsync(CacheSource source = CacheSource.Manual,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Charge les donnees initiales depuis le fichier JSON seed.
    /// </summary>
    Task LoadFromSeedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalide le cache (supprime L1 et L2).
    /// </summary>
    Task InvalidateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cle unique du cache.
    /// </summary>
    string CacheKey { get; }
}
