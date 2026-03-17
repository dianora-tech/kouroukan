using System.ComponentModel.DataAnnotations;

namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Requete de connexion.
/// </summary>
public class LoginRequest
{
    /// <summary>Email de l'utilisateur.</summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Mot de passe.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
