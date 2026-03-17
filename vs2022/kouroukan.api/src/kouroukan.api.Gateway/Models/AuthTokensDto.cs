namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// DTO retourne apres authentification ou rafraichissement de tokens.
/// </summary>
public class AuthTokensDto
{
    /// <summary>Token JWT d'acces.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Token opaque de rafraichissement.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>Date d'expiration du access token (UTC).</summary>
    public DateTime AccessTokenExpiresAt { get; set; }

    /// <summary>Date d'expiration du refresh token (UTC).</summary>
    public DateTime RefreshTokenExpiresAt { get; set; }
}
