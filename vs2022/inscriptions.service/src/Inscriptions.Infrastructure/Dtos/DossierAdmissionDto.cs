using GnDapper.Entities;

namespace Inscriptions.Infrastructure.Dtos;

public sealed class DossierAdmissionDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public int EleveId { get; set; }
    public int AnneeScolaireId { get; set; }
    public string StatutDossier { get; set; } = string.Empty;
    public string EtapeActuelle { get; set; } = string.Empty;
    public DateTime DateDemande { get; set; }
    public DateTime? DateDecision { get; set; }
    public string? MotifRefus { get; set; }
    public decimal? ScoringInterne { get; set; }
    public string? Commentaires { get; set; }
    public int? ResponsableAdmissionId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
