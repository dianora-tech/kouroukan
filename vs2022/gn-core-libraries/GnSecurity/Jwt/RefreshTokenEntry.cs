namespace GnSecurity.Jwt;

/// <summary>
/// Represente un refresh token stocke en base de donnees.
/// </summary>
public class RefreshTokenEntry
{
    /// <summary>Identifiant de l'utilisateur proprietaire du token.</summary>
    public int UserId { get; set; }

    /// <summary>Valeur du refresh token (opaque).</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Date d'expiration du token (UTC).</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Indique si le token a ete revoque.</summary>
    public bool IsRevoked { get; set; }

    /// <summary>Date de revocation (UTC), null si non revoque.</summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>Token de remplacement apres rotation, null si non remplace.</summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>Date de creation du token (UTC).</summary>
    public DateTime CreatedAt { get; set; }
}
