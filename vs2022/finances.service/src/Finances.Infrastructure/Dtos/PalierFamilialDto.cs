using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Finances.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table finances.paliers_familiaux.
/// </summary>
[Table("finances.paliers_familiaux")]
public sealed class PalierFamilialDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int RangEnfant { get; set; }
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
