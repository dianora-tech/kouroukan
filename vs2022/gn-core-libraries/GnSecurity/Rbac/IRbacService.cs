namespace GnSecurity.Rbac;

/// <summary>
/// Service RBAC (Role-Based Access Control) avec cache memoire.
/// </summary>
public interface IRbacService
{
    /// <summary>
    /// Verifie si un utilisateur possede une permission specifique.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="permission">Nom de la permission (ex: "inscriptions:read").</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns><c>true</c> si l'utilisateur possede la permission.</returns>
    Task<bool> HasPermissionAsync(int userId, string permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifie si un utilisateur possede un role specifique.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="role">Nom du role (ex: "directeur").</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns><c>true</c> si l'utilisateur possede le role.</returns>
    Task<bool> HasRoleAsync(int userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupere toutes les permissions d'un utilisateur.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des noms de permissions.</returns>
    Task<IReadOnlyList<string>> GetPermissionsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigne un role a un utilisateur et invalide le cache.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="role">Nom du role a assigner.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task AssignRoleAsync(int userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoque un role d'un utilisateur et invalide le cache.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="role">Nom du role a revoquer.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task RevokeRoleAsync(int userId, string role, CancellationToken cancellationToken = default);
}
