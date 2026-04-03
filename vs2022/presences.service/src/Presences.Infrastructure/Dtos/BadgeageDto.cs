using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Presences.Infrastructure.Dtos;

[Table("presences.badgeages")]
public sealed class BadgeageDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public int EleveId { get; set; }
    public DateTime DateBadgeage { get; set; }
    public TimeSpan HeureBadgeage { get; set; }
    public string PointAcces { get; set; } = string.Empty;
    public string MethodeBadgeage { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
