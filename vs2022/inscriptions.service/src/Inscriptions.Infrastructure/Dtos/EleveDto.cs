using GnDapper.Attributes;
using GnDapper.Entities;

namespace Inscriptions.Infrastructure.Dtos;

[Table("inscriptions.eleves")]
public sealed class EleveDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }
    public string LieuNaissance { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Nationalite { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? PhotoUrl { get; set; }
    public string NumeroMatricule { get; set; } = string.Empty;
    public int NiveauClasseId { get; set; }
    public int? ClasseId { get; set; }
    public int? ParentId { get; set; }
    public string StatutInscription { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
