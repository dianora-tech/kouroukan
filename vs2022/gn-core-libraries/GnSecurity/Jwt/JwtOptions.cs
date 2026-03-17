namespace GnSecurity.Jwt;

/// <summary>
/// Options de configuration pour la generation et validation des tokens JWT.
/// Section de configuration : "Jwt".
/// </summary>
public class JwtOptions
{
    /// <summary>Cle secrete HMAC-SHA512 (minimum 64 caracteres recommandes).</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Emetteur du token (iss claim).</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Audience du token (aud claim).</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Duree de validite du access token en minutes. Defaut : 15.</summary>
    public int AccessTokenDurationMinutes { get; set; } = 15;

    /// <summary>Duree de validite du refresh token en jours. Defaut : 7.</summary>
    public int RefreshTokenDurationDays { get; set; } = 7;
}
