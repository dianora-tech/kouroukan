namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// DTO pour un utilisateur dans la liste de l'etablissement.
/// </summary>
public class UserListItemDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO retourne apres creation d'un utilisateur (inclut le mot de passe temporaire).
/// </summary>
public class CreateUserResultDto
{
    public int UserId { get; set; }
    public string TemporaryPassword { get; set; } = string.Empty;
}

/// <summary>
/// DTO pour la recherche d'utilisateur (fondateur).
/// </summary>
public class UserSearchResultDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// DTO pour un etablissement lie a un utilisateur.
/// </summary>
public class CompanyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    /// <summary>Modules souscrits par l'etablissement.</summary>
    public string[]? Modules { get; set; }
}
