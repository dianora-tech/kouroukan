using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Finances.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table finances.journal_financier.
/// </summary>
[Table("finances.journal_financier")]
public sealed class JournalFinancierDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReferenceExterne { get; set; }
    public string ModePaiement { get; set; } = string.Empty;
    public DateTime DateOperation { get; set; }
    public int? EleveId { get; set; }
    public int? ParentUserId { get; set; }

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
