using GnDapper.Attributes;
using GnDapper.Entities;

namespace Pedagogie.Infrastructure.Dtos;

[Table("pedagogie.niveaux_classes")]
public sealed class NiveauClasseDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Ordre { get; set; }
    public string CycleEtude { get; set; } = string.Empty;
    public int? AgeOfficielEntree { get; set; }
    public string? MinistereTutelle { get; set; }
    public string? ExamenSortie { get; set; }
    public decimal? TauxHoraireEnseignant { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
