namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// DTO du profil utilisateur retourne par GET /api/auth/me.
/// </summary>
public class UserProfileDto
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

    /// <summary>Roles de l'utilisateur.</summary>
    public IReadOnlyList<string> Roles { get; set; } = [];

    /// <summary>Permissions de l'utilisateur.</summary>
    public IReadOnlyList<string> Permissions { get; set; } = [];

    /// <summary>Version des CGU acceptees.</summary>
    public string? CguVersion { get; set; }

    /// <summary>Date d'acceptation des CGU.</summary>
    public DateTime? CguAcceptedAt { get; set; }
}
