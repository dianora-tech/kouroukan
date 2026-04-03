using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Bde.Infrastructure.Dtos;

[Table("bde.membres_bde")]
public sealed class MembreBdeDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int AssociationId { get; set; }
    public int EleveId { get; set; }
    public string RoleBde { get; set; } = string.Empty;
    public DateTime DateAdhesion { get; set; }
    public decimal? MontantCotisation { get; set; }
    public bool EstActif { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
