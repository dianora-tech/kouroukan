namespace GnSecurity.Rbac;

/// <summary>
/// Represente une permission applicative (ex: "inscriptions:read").
/// </summary>
public class Permission
{
    /// <summary>Identifiant unique de la permission.</summary>
    public int Id { get; set; }

    /// <summary>Nom unique de la permission (ex: "inscriptions:read").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description de la permission.</summary>
    public string? Description { get; set; }

    /// <summary>Module auquel appartient la permission.</summary>
    public string? Module { get; set; }
}
