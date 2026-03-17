namespace GnSecurity.Rbac;

/// <summary>
/// Represente un role applicatif (ex: "directeur", "enseignant").
/// </summary>
public class Role
{
    /// <summary>Identifiant unique du role.</summary>
    public int Id { get; set; }

    /// <summary>Nom unique du role (ex: "directeur").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description du role.</summary>
    public string? Description { get; set; }
}
