using System.ComponentModel.DataAnnotations;

namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Requete de mise a jour des preferences utilisateur.
/// </summary>
public class UpdatePreferencesRequest
{
    /// <summary>Langue preferee (fr, en).</summary>
    [Required]
    [RegularExpression("^(fr|en)$", ErrorMessage = "La langue doit etre 'fr' ou 'en'.")]
    public string Locale { get; set; } = "fr";

    /// <summary>Theme prefere (light, dark, system).</summary>
    [Required]
    [RegularExpression("^(light|dark|system)$", ErrorMessage = "Le theme doit etre 'light', 'dark' ou 'system'.")]
    public string Theme { get; set; } = "system";
}
