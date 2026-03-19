using System.ComponentModel.DataAnnotations;

namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Requete de creation d'un utilisateur par le directeur.
/// </summary>
public class CreateUserRequest
{
    [Required]
    [MinLength(2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\+224\s?\d{3}\s?\d{2}\s?\d{2}\s?\d{2}$",
        ErrorMessage = "Le numero de telephone doit etre au format +224 XXX XX XX XX.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>Nom du role a assigner (enseignant, censeur, fondateur, etc.).</summary>
    [Required]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// ID d'un utilisateur existant a lier (pour le fondateur multi-etablissement).
    /// Si fourni, aucun nouveau compte n'est cree.
    /// </summary>
    public int? ExistingUserId { get; set; }
}
