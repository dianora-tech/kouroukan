using Dapper;
using GnDapper.Connection;
using GnSecurity.Rbac;

namespace Kouroukan.Api.Gateway.Auth;

/// <summary>
/// Implementation de <see cref="IPermissionStore"/> via Dapper + PostgreSQL.
/// Gere les roles et permissions dans les tables auth.
/// </summary>
public sealed class PermissionStore : IPermissionStore
{
    private readonly IDbConnectionFactory _connectionFactory;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="PermissionStore"/>.
    /// </summary>
    public PermissionStore(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetRolesForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var roles = await connection.QueryAsync<string>(
            """
            SELECT r.name
            FROM auth.user_roles ur
            INNER JOIN auth.roles r ON r.id = ur.role_id
            WHERE ur.user_id = @UserId
              AND ur.is_deleted = FALSE
              AND r.is_deleted = FALSE
            """,
            new { UserId = userId });

        return roles.ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetPermissionsForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var permissions = await connection.QueryAsync<string>(
            """
            SELECT DISTINCT p.name
            FROM auth.user_roles ur
            INNER JOIN auth.role_permissions rp ON rp.role_id = ur.role_id
            INNER JOIN auth.permissions p ON p.id = rp.permission_id
            WHERE ur.user_id = @UserId
              AND ur.is_deleted = FALSE
              AND rp.is_deleted = FALSE
              AND p.is_deleted = FALSE
            """,
            new { UserId = userId });

        return permissions.ToList();
    }

    /// <inheritdoc />
    public async Task AssignRoleAsync(int userId, string role, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            INSERT INTO auth.user_roles (user_id, role_id, created_by)
            SELECT @UserId, r.id, 'system'
            FROM auth.roles r
            WHERE r.name = @Role AND r.is_deleted = FALSE
            ON CONFLICT (user_id, role_id) DO NOTHING
            """,
            new { UserId = userId, Role = role });
    }

    /// <inheritdoc />
    public async Task RevokeRoleAsync(int userId, string role, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            UPDATE auth.user_roles
            SET is_deleted = TRUE, deleted_at = NOW(), deleted_by = 'system'
            WHERE user_id = @UserId
              AND role_id = (SELECT id FROM auth.roles WHERE name = @Role AND is_deleted = FALSE)
              AND is_deleted = FALSE
            """,
            new { UserId = userId, Role = role });
    }
}
