using GnDapper.Entities;

namespace Inscriptions.Domain.Entities;

/// <summary>
/// Liaison entre un parent et un eleve au sein d'un etablissement.
/// Table : inscriptions.liaisons_parent
/// </summary>
public class LiaisonParent : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers l'utilisateur parent (auth.users).</summary>
    public int ParentUserId { get; set; }

    /// <summary>FK vers l'eleve (inscriptions.eleves).</summary>
    public int EleveId { get; set; }

    /// <summary>FK vers l'etablissement (auth.companies).</summary>
    public int CompanyId { get; set; }

    /// <summary>Statut de la liaison : Active, Inactive.</summary>
    public string Statut { get; set; } = string.Empty;

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }

    // ISoftDeletable
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
