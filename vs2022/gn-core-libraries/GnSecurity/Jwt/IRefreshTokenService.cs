namespace GnSecurity.Jwt;

/// <summary>
/// Service de gestion des refresh tokens avec rotation et detection de reutilisation.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Rafraichit les tokens d'un utilisateur a partir d'un refresh token valide.
    /// Implemente la rotation de tokens : le refresh token utilise est revoque et remplace.
    /// Detecte la reutilisation : si un token deja revoque est presente, tous les tokens
    /// de l'utilisateur sont revoques (possible vol de token).
    /// </summary>
    /// <param name="refreshToken">Refresh token a utiliser pour le rafraichissement.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Un nouveau <see cref="TokenResult"/> contenant les nouveaux tokens.</returns>
    /// <exception cref="InvalidOperationException">
    /// Si le token est invalide, expire, revoque (avec revocation en chaine) ou si l'utilisateur est introuvable.
    /// </exception>
    Task<TokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoque un refresh token specifique.
    /// </summary>
    /// <param name="refreshToken">Refresh token a revoquer.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}
