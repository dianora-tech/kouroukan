using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GnSecurity.Rbac;

/// <summary>
/// Service RBAC (Role-Based Access Control) avec cache memoire.
/// Les permissions et roles sont mis en cache pendant 5 minutes (expiration glissante)
/// pour minimiser les appels en base de donnees.
/// </summary>
public sealed class RbacService : IRbacService
{
    /// <summary>Prefixe des cles de cache pour les permissions.</summary>
    private const string PermissionsCachePrefix = "rbac:permissions:";

    /// <summary>Prefixe des cles de cache pour les roles.</summary>
    private const string RolesCachePrefix = "rbac:roles:";

    /// <summary>Duree d'expiration glissante du cache (5 minutes).</summary>
    private static readonly TimeSpan CacheSlidingExpiration = TimeSpan.FromMinutes(5);

    private readonly IPermissionStore _store;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RbacService> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="RbacService"/>.
    /// </summary>
    /// <param name="store">Store de persistance des roles et permissions.</param>
    /// <param name="cache">Cache memoire pour les donnees RBAC.</param>
    /// <param name="logger">Logger pour les evenements de securite.</param>
    public RbacService(
        IPermissionStore store,
        IMemoryCache cache,
        ILogger<RbacService> logger)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> HasPermissionAsync(int userId, string permission, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission, nameof(permission));

        var permissions = await GetPermissionsAsync(userId, cancellationToken).ConfigureAwait(false);
        var hasPermission = permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);

        if (!hasPermission)
        {
            _logger.LogDebug(
                "Utilisateur {UserId} n'a pas la permission '{Permission}'.",
                userId, permission);
        }

        return hasPermission;
    }

    /// <inheritdoc />
    public async Task<bool> HasRoleAsync(int userId, string role, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role, nameof(role));

        var roles = await GetRolesInternalAsync(userId, cancellationToken).ConfigureAwait(false);
        var hasRole = roles.Contains(role, StringComparer.OrdinalIgnoreCase);

        if (!hasRole)
        {
            _logger.LogDebug(
                "Utilisateur {UserId} n'a pas le role '{Role}'.",
                userId, role);
        }

        return hasRole;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{PermissionsCachePrefix}{userId}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<string>? cachedPermissions) && cachedPermissions is not null)
        {
            return cachedPermissions;
        }

        var permissions = await _store.GetPermissionsForUserAsync(userId, cancellationToken).ConfigureAwait(false);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = CacheSlidingExpiration
        };

        _cache.Set(cacheKey, permissions, cacheOptions);

        _logger.LogDebug(
            "Permissions chargees depuis le store pour l'utilisateur {UserId} : {Count} permission(s).",
            userId, permissions.Count);

        return permissions;
    }

    /// <inheritdoc />
    public async Task AssignRoleAsync(int userId, string role, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role, nameof(role));

        await _store.AssignRoleAsync(userId, role, cancellationToken).ConfigureAwait(false);

        InvalidateCacheForUser(userId);

        _logger.LogInformation(
            "Role '{Role}' assigne a l'utilisateur {UserId}. Cache invalide.",
            role, userId);
    }

    /// <inheritdoc />
    public async Task RevokeRoleAsync(int userId, string role, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role, nameof(role));

        await _store.RevokeRoleAsync(userId, role, cancellationToken).ConfigureAwait(false);

        InvalidateCacheForUser(userId);

        _logger.LogInformation(
            "Role '{Role}' revoque pour l'utilisateur {UserId}. Cache invalide.",
            role, userId);
    }

    /// <summary>
    /// Recupere les roles d'un utilisateur avec mise en cache.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des noms de roles.</returns>
    private async Task<IReadOnlyList<string>> GetRolesInternalAsync(int userId, CancellationToken cancellationToken)
    {
        var cacheKey = $"{RolesCachePrefix}{userId}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<string>? cachedRoles) && cachedRoles is not null)
        {
            return cachedRoles;
        }

        var roles = await _store.GetRolesForUserAsync(userId, cancellationToken).ConfigureAwait(false);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = CacheSlidingExpiration
        };

        _cache.Set(cacheKey, roles, cacheOptions);

        _logger.LogDebug(
            "Roles charges depuis le store pour l'utilisateur {UserId} : {Count} role(s).",
            userId, roles.Count);

        return roles;
    }

    /// <summary>
    /// Invalide les entrees de cache (roles et permissions) pour un utilisateur.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    private void InvalidateCacheForUser(int userId)
    {
        _cache.Remove($"{PermissionsCachePrefix}{userId}");
        _cache.Remove($"{RolesCachePrefix}{userId}");
    }
}
