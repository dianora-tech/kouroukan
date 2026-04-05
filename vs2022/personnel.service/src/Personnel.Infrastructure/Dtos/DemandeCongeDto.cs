using GnDapper.Attributes;
using GnDapper.Entities;

namespace Personnel.Infrastructure.Dtos;

[Table("personnel.demandes_conges")]
public sealed class DemandeCongeDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int EnseignantId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Motif { get; set; } = string.Empty;
    public string StatutDemande { get; set; } = string.Empty;
    public string? PieceJointeUrl { get; set; }
    public string? CommentaireValidateur { get; set; }
    public int? ValidateurId { get; set; }
    public DateTime? DateValidation { get; set; }
    public bool ImpactPaie { get; set; }
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
