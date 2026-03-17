using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Finances.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table finances.factures.
/// </summary>
[Table("finances.factures")]
public sealed class FactureDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public int EleveId { get; set; }
    public int? ParentId { get; set; }
    public int AnneeScolaireId { get; set; }
    public decimal MontantTotal { get; set; }
    public decimal MontantPaye { get; set; }
    public decimal Solde { get; set; }
    public DateTime DateEmission { get; set; }
    public DateTime DateEcheance { get; set; }
    public string StatutFacture { get; set; } = string.Empty;
    public string NumeroFacture { get; set; } = string.Empty;
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
