namespace GnSecurity.Rbac;

/// <summary>
/// Represente l'association entre un role et une permission.
/// </summary>
public class RolePermissionMapping
{
    /// <summary>Identifiant du role.</summary>
    public int RoleId { get; set; }

    /// <summary>Nom du role.</summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>Identifiant de la permission.</summary>
    public int PermissionId { get; set; }

    /// <summary>Nom de la permission.</summary>
    public string PermissionName { get; set; } = string.Empty;
}
