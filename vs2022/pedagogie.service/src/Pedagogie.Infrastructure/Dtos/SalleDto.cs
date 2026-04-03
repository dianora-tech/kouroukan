using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Pedagogie.Infrastructure.Dtos;

[Table("pedagogie.salles")]
public sealed class SalleDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public int Capacite { get; set; }
    public string? Batiment { get; set; }
    public string? Equipements { get; set; }
    public bool EstDisponible { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
