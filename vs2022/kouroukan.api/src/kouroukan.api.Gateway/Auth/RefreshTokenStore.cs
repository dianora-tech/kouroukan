using Dapper;
using GnDapper.Connection;
using GnSecurity.Jwt;

namespace Kouroukan.Api.Gateway.Auth;

/// <summary>
/// Implementation de <see cref="IRefreshTokenStore"/> via Dapper + PostgreSQL.
/// Persiste les refresh tokens dans la table auth.refresh_tokens.
/// </summary>
public sealed class RefreshTokenStore : IRefreshTokenStore
{
    private readonly IDbConnectionFactory _connectionFactory;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="RefreshTokenStore"/>.
    /// </summary>
    public RefreshTokenStore(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public async Task StoreAsync(int userId, string token, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            INSERT INTO auth.refresh_tokens (user_id, token, expires_at, is_revoked, created_at)
            VALUES (@UserId, @Token, @ExpiresAt, FALSE, NOW())
            """,
            new { UserId = userId, Token = token, ExpiresAt = expiresAt });
    }

    /// <inheritdoc />
    public async Task<RefreshTokenEntry?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<RefreshTokenEntry>(
            """
            SELECT user_id, token, expires_at, is_revoked, revoked_at, replaced_by_token, created_at
            FROM auth.refresh_tokens
            WHERE token = @Token
            """,
            new { Token = token });
    }

    /// <inheritdoc />
    public async Task RevokeAsync(string token, string? replacedByToken = null, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            UPDATE auth.refresh_tokens
            SET is_revoked = TRUE, revoked_at = NOW(), replaced_by_token = @ReplacedByToken
            WHERE token = @Token
            """,
            new { Token = token, ReplacedByToken = replacedByToken });
    }

    /// <inheritdoc />
    public async Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            UPDATE auth.refresh_tokens
            SET is_revoked = TRUE, revoked_at = NOW()
            WHERE user_id = @UserId AND is_revoked = FALSE
            """,
            new { UserId = userId });
    }
}
