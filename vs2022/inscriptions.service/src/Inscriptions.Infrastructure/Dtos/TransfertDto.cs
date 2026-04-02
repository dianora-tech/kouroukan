using GnDapper.Entities;

namespace Inscriptions.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table inscriptions.transferts.
/// </summary>
public sealed class TransfertDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int EleveId { get; set; }
    public int CompanyOrigineId { get; set; }
    public int CompanyCibleId { get; set; }
    public string Statut { get; set; } = string.Empty;
    public string Motif { get; set; } = string.Empty;
    public string? Documents { get; set; }
    public DateTime DateDemande { get; set; }
    public DateTime? DateTraitement { get; set; }

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
