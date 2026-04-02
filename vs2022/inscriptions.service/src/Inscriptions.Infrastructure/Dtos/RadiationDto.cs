using GnDapper.Attributes;
using GnDapper.Entities;

namespace Inscriptions.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table inscriptions.radiations.
/// </summary>
[Table("inscriptions.radiations")]
public sealed class RadiationDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int EleveId { get; set; }
    public int CompanyId { get; set; }
    public string Motif { get; set; } = string.Empty;
    public string? Documents { get; set; }
    public DateTime DateRadiation { get; set; }

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
