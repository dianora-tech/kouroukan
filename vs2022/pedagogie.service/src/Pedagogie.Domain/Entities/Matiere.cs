using GnDapper.Entities;

namespace Pedagogie.Domain.Entities;

/// <summary>
/// Matiere enseignee dans un niveau de classe.
/// Table : pedagogie.matieres
/// </summary>
public class Matiere : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom de la matiere (ex: "Mathematiques").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description de la matiere.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers le type de matiere.</summary>
    public int TypeId { get; set; }

    /// <summary>Code matiere (MATH, FR, HG...).</summary>
    public string Code { get; set; } = string.Empty;

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
