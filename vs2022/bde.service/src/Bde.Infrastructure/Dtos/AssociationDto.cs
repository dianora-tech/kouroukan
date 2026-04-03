using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Bde.Infrastructure.Dtos;

[Table("bde.associations")]
public sealed class AssociationDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Sigle { get; set; }
    public string AnneeScolaire { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public decimal BudgetAnnuel { get; set; }
    public int? SuperviseurId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
