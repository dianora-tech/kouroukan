using GnDapper.Entities;

namespace Bde.Infrastructure.Dtos;

public sealed class DepenseBdeDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AssociationId { get; set; }
    public decimal Montant { get; set; }
    public string Motif { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string StatutValidation { get; set; } = string.Empty;
    public int? ValidateurId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
