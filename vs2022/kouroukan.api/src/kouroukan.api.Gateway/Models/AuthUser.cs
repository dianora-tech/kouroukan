namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Represente un utilisateur de la table auth.users.
/// </summary>
public class AuthUser
{
    /// <summary>Identifiant unique.</summary>
    public int Id { get; set; }

    /// <summary>Prenom.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Nom de famille.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Email.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Numero de telephone.</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>Hash du mot de passe (Argon2id).</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Compte actif.</summary>
    public bool IsActive { get; set; }

    /// <summary>Date de derniere connexion.</summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>Date d'acceptation des CGU.</summary>
    public DateTime? CguAcceptedAt { get; set; }

    /// <summary>Version des CGU acceptees.</summary>
    public string? CguVersion { get; set; }

    /// <summary>Doit changer le mot de passe a la prochaine connexion.</summary>
    public bool MustChangePassword { get; set; }

    /// <summary>Suppression logique.</summary>
    public bool IsDeleted { get; set; }
}
