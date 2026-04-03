using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Documents.Infrastructure.Dtos;

[Table("documents.documents_generes")]
public sealed class DocumentGenereDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public int ModeleDocumentId { get; set; }
    public int? EleveId { get; set; }
    public int? EnseignantId { get; set; }
    public string DonneesJson { get; set; } = string.Empty;
    public DateTime DateGeneration { get; set; }
    public string StatutSignature { get; set; } = string.Empty;
    public string? CheminFichier { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
