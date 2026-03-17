using GnDapper.Entities;

namespace Communication.Infrastructure.Dtos;

public sealed class MessageDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TypeId { get; set; }
    public int ExpediteurId { get; set; }
    public int? DestinataireId { get; set; }
    public string Sujet { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public bool EstLu { get; set; }
    public DateTime? DateLecture { get; set; }
    public string? GroupeDestinataire { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
