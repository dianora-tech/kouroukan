using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Finances.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table finances.paiements.
/// </summary>
[Table("finances.paiements")]
public sealed class PaiementDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public int FactureId { get; set; }
    public decimal MontantPaye { get; set; }
    public DateTime DatePaiement { get; set; }
    public string MoyenPaiement { get; set; } = string.Empty;
    public string? ReferenceMobileMoney { get; set; }
    public string StatutPaiement { get; set; } = string.Empty;
    public int? CaissierId { get; set; }
    public string NumeroRecu { get; set; } = string.Empty;
    public int UserId { get; set; }

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
