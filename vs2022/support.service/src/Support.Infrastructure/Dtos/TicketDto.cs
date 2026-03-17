using GnDapper.Entities;

namespace Support.Infrastructure.Dtos;

public sealed class TicketDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TypeId { get; set; }
    public int AuteurId { get; set; }
    public string Sujet { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public string Priorite { get; set; } = string.Empty;
    public string StatutTicket { get; set; } = string.Empty;
    public string CategorieTicket { get; set; } = string.Empty;
    public string? ModuleConcerne { get; set; }
    public int? AssigneAId { get; set; }
    public DateTime? DateResolution { get; set; }
    public int? NoteSatisfaction { get; set; }
    public string? PieceJointeUrl { get; set; }
    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
