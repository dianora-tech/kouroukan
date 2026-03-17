namespace GnSecurity.Jwt;

/// <summary>
/// Resultat de generation de tokens contenant l'access token et le refresh token.
/// </summary>
/// <param name="AccessToken">Token JWT d'acces.</param>
/// <param name="RefreshToken">Token opaque de rafraichissement.</param>
/// <param name="AccessTokenExpiresAt">Date d'expiration du access token (UTC).</param>
/// <param name="RefreshTokenExpiresAt">Date d'expiration du refresh token (UTC).</param>
public record TokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt
);
