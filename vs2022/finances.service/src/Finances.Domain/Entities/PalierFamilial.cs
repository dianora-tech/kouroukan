using GnDapper.Entities;

namespace Finances.Domain.Entities;

/// <summary>
/// Palier de reduction familiale selon le rang de l'enfant.
/// Table : finances.paliers_familiaux
/// </summary>
public class PalierFamilial : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers l'etablissement (auth.companies).</summary>
    public int CompanyId { get; set; }

    /// <summary>Rang de l'enfant dans la famille (2, 3, 4...).</summary>
    public int RangEnfant { get; set; }

    /// <summary>Pourcentage de reduction applique.</summary>
    public decimal ReductionPourcent { get; set; }

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
