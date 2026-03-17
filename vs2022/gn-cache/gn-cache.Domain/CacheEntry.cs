namespace GnCache.Domain;

/// <summary>
/// Represente une entree dans le cache avec ses metadonnees.
/// </summary>
/// <typeparam name="T">Type des donnees mises en cache.</typeparam>
public sealed class CacheEntry<T> where T : class
{
    /// <summary>
    /// Cle unique du cache (ex: "regions", "matieres").
    /// </summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Donnees mises en cache.
    /// </summary>
    public IReadOnlyList<T> Data { get; init; } = [];

    /// <summary>
    /// Date de derniere mise a jour (UTC).
    /// </summary>
    public DateTime LastRefreshedAtUtc { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Source du dernier chargement.
    /// </summary>
    public CacheSource Source { get; init; }

    /// <summary>
    /// Nombre d'elements dans le cache.
    /// </summary>
    public int Count => Data.Count;
}
