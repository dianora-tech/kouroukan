using GnDapper.Attributes;
using GnDapper.Entities;

namespace Communication.Infrastructure.Dtos;

[Table("communication.notifications")]
public sealed class NotificationDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public string DestinatairesIds { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public string Canal { get; set; } = string.Empty;
    public bool EstEnvoyee { get; set; }
    public DateTime? DateEnvoi { get; set; }
    public string? LienAction { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
