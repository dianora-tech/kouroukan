namespace GnSecurity.Rbac;

/// <summary>
/// Interface de persistance des roles et permissions.
/// A implementer par le consommateur (ex: via Dapper + PostgreSQL sur auth.user_roles / auth.role_permissions).
/// </summary>
public interface IPermissionStore
{
    /// <summary>
    /// Recupere les noms de roles assignes a un utilisateur.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des noms de roles.</returns>
    Task<IReadOnlyList<string>> GetRolesForUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupere les noms de permissions d'un utilisateur (via ses roles).
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des noms de permissions (dedupliques).</returns>
    Task<IReadOnlyList<string>> GetPermissionsForUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigne un role a un utilisateur en base de donnees.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="role">Nom du role a assigner.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task AssignRoleAsync(int userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoque un role d'un utilisateur en base de donnees.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="role">Nom du role a revoquer.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task RevokeRoleAsync(int userId, string role, CancellationToken cancellationToken = default);
}
