using GnDapper.Attributes;
using GnDapper.Entities;

namespace Inscriptions.Infrastructure.Dtos;

[Table("inscriptions.inscriptions")]
public sealed class InscriptionDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public int EleveId { get; set; }
    public int ClasseId { get; set; }
    public int AnneeScolaireId { get; set; }
    public DateTime DateInscription { get; set; }
    public decimal MontantInscription { get; set; }
    public bool EstPaye { get; set; }
    public bool EstRedoublant { get; set; }
    public string? TypeEtablissement { get; set; }
    public string? SerieBac { get; set; }
    public string StatutInscription { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
