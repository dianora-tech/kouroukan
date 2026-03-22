using System.ComponentModel.DataAnnotations;

namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Requete d'inscription d'un nouvel etablissement (etape wizard du portail).
/// </summary>
public class RegisterRequest
{
    /// <summary>Prenom du directeur.</summary>
    [Required]
    [MinLength(2)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Nom de famille du directeur.</summary>
    [Required]
    [MinLength(2)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Numero de telephone guineen (9 chiffres, ex: 629817970).</summary>
    [Required]
    [RegularExpression(@"^(\+224)?\s?\d{9}$",
        ErrorMessage = "Le numero de telephone doit contenir 9 chiffres (ex: 629817970 ou +224629817970).")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>Adresse email (optionnelle).</summary>
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>Mot de passe en clair (sera hache cote serveur).</summary>
    [Required]
    [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caracteres.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Modules souscrits (au moins un requis).</summary>
    [Required]
    [MinLength(1, ErrorMessage = "Au moins un module doit etre selectionne.")]
    public List<string> Modules { get; set; } = [];

    /// <summary>Nom de l'etablissement. Par defaut : Prenom + Nom du directeur.</summary>
    public string? SchoolName { get; set; }

    /// <summary>Code de la region (ex: 'CKY', 'BOK', ...).</summary>
    public string? Region { get; set; }

    /// <summary>Code de la prefecture.</summary>
    public string? Prefecture { get; set; }

    /// <summary>Code de la sous-prefecture. Absent pour Conakry (zones speciales).</summary>
    public string? SousPrefecture { get; set; }

    /// <summary>Adresse libre de l'etablissement.</summary>
    public string? Address { get; set; }
}
