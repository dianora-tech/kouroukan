using GnCache.Domain;

namespace GnCache.Application.Services;

/// <summary>
/// Registre central de toutes les entites de cache declarees.
/// Permet de resoudre dynamiquement les caches par cle.
/// </summary>
public interface ICacheRegistry
{
    /// <summary>
    /// Recupere toutes les entites de cache enregistrees.
    /// </summary>
    IReadOnlyList<CacheEntityRegistration> GetAllRegistrations();

    /// <summary>
    /// Recupere l'enregistrement d'un cache par sa cle.
    /// </summary>
    CacheEntityRegistration? GetRegistration(string cacheKey);

    /// <summary>
    /// Recharge tous les caches enregistres.
    /// </summary>
    Task ReloadAllAsync(CacheSource source = CacheSource.Manual,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recharge un cache specifique par sa cle.
    /// </summary>
    Task ReloadAsync(string cacheKey, CacheSource source = CacheSource.Manual,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupere les donnees d'un cache par sa cle (retourne object).
    /// </summary>
    Task<object?> GetDataAsync(string cacheKey,
        CancellationToken cancellationToken = default);
}
