using GnDapper.Attributes;
using GnDapper.Entities;

namespace Personnel.Infrastructure.Dtos;

[Table("personnel.enseignants")]
public sealed class EnseignantDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Matricule { get; set; } = string.Empty;
    public string Specialite { get; set; } = string.Empty;
    public DateTime DateEmbauche { get; set; }
    public string ModeRemuneration { get; set; } = string.Empty;
    public decimal? MontantForfait { get; set; }
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string StatutEnseignant { get; set; } = string.Empty;
    public int SoldeCongesAnnuel { get; set; }
    public int TypeId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
