using Dapper;
using GnDapper.Connection;
using GnSecurity.Jwt;

namespace Kouroukan.Api.Gateway.Auth;

/// <summary>
/// Implementation de <see cref="IUserClaimsProvider"/> via Dapper + PostgreSQL.
/// Fournit les claims utilisateur pour le rafraichissement de tokens.
/// </summary>
public sealed class UserClaimsProvider : IUserClaimsProvider
{
    private readonly IDbConnectionFactory _connectionFactory;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="UserClaimsProvider"/>.
    /// </summary>
    public UserClaimsProvider(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public async Task<UserClaims?> GetUserClaimsAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var user = await connection.QuerySingleOrDefaultAsync<dynamic>(
            """
            SELECT id, first_name, last_name, email, is_active
            FROM auth.users
            WHERE id = @UserId AND is_deleted = FALSE AND is_active = TRUE
            """,
            new { UserId = userId });

        if (user is null) return null;

        var roles = (await connection.QueryAsync<string>(
            """
            SELECT r.name
            FROM auth.user_roles ur
            INNER JOIN auth.roles r ON r.id = ur.role_id
            WHERE ur.user_id = @UserId
              AND ur.is_deleted = FALSE
              AND r.is_deleted = FALSE
            """,
            new { UserId = userId })).ToList();

        var permissions = (await connection.QueryAsync<string>(
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
            new { UserId = userId })).ToList();

        return new UserClaims(
            userId,
            (string)user.email,
            $"{(string)user.first_name} {(string)user.last_name}",
            roles,
            permissions);
    }
}
