using GnDapper.Entities;

namespace Support.Infrastructure.Dtos;

public sealed class ReponseTicketDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TicketId { get; set; }
    public int AuteurId { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public bool EstReponseIA { get; set; }
    public bool EstInterne { get; set; }
    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
