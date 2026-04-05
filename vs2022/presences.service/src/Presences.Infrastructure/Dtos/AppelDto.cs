using GnDapper.Attributes;
using GnDapper.Entities;

namespace Presences.Infrastructure.Dtos;

[Table("presences.appels")]
public sealed class AppelDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int ClasseId { get; set; }
    public int EnseignantId { get; set; }
    public int? SeanceId { get; set; }
    public DateTime DateAppel { get; set; }
    public TimeSpan HeureAppel { get; set; }
    public bool EstCloture { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
