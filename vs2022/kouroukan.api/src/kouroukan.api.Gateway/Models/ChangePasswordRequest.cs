using System.ComponentModel.DataAnnotations;

namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Requete de changement de mot de passe.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>Mot de passe actuel (temporaire ou ancien).</summary>
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>Nouveau mot de passe choisi par l'utilisateur.</summary>
    [Required]
    [MinLength(8, ErrorMessage = "Le nouveau mot de passe doit contenir au moins 8 caracteres.")]
    public string NewPassword { get; set; } = string.Empty;
}
