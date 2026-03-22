namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Requete de deconnexion. Tous les champs sont optionnels.
/// </summary>
public class LogoutRequest
{
    /// <summary>Refresh token a revoquer (optionnel).</summary>
    public string? RefreshToken { get; set; }
}
