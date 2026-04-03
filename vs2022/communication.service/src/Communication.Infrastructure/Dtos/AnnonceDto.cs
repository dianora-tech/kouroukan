using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Communication.Infrastructure.Dtos;

[Table("communication.annonces")]
public sealed class AnnonceDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public bool EstActive { get; set; }
    public string CibleAudience { get; set; } = string.Empty;
    public int Priorite { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
