using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Documents.Infrastructure.Dtos;

[Table("documents.modeles_documents")]
public sealed class ModeleDocumentDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? CouleurPrimaire { get; set; }
    public string? TextePiedPage { get; set; }
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
