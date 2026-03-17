namespace GnSecurity.Jwt;

/// <summary>
/// Interface de persistance des refresh tokens.
/// A implementer par le consommateur (ex: via Dapper + PostgreSQL).
/// </summary>
public interface IRefreshTokenStore
{
    /// <summary>
    /// Stocke un nouveau refresh token.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="token">Valeur du refresh token.</param>
    /// <param name="expiresAt">Date d'expiration (UTC).</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task StoreAsync(int userId, string token, DateTime expiresAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupere un refresh token par sa valeur.
    /// </summary>
    /// <param name="token">Valeur du refresh token.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>L'entree du token, ou <c>null</c> si non trouve.</returns>
    Task<RefreshTokenEntry?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoque un refresh token specifique et optionnellement enregistre le token de remplacement.
    /// </summary>
    /// <param name="token">Valeur du refresh token a revoquer.</param>
    /// <param name="replacedByToken">Token de remplacement (rotation), null si simple revocation.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task RevokeAsync(string token, string? replacedByToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoque tous les refresh tokens d'un utilisateur (detection de reutilisation).
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken = default);
}
