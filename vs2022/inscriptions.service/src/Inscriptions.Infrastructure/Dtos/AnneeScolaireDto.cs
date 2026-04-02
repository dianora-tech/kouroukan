using GnDapper.Attributes;
using GnDapper.Entities;

namespace Inscriptions.Infrastructure.Dtos;

[Table("inscriptions.annees_scolaires")]
public sealed class AnneeScolaireDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public bool EstActive { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string Statut { get; set; } = "preparation";
    public DateTime? DateRentree { get; set; }
    public int NombrePeriodes { get; set; } = 3;
    public string TypePeriode { get; set; } = "trimestre";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
