using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Documents.Infrastructure.Dtos;

[Table("documents.signatures")]
public sealed class SignatureDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public int DocumentGenereId { get; set; }
    public int SignataireId { get; set; }
    public int OrdreSignature { get; set; }
    public DateTime? DateSignature { get; set; }
    public string StatutSignature { get; set; } = string.Empty;
    public string NiveauSignature { get; set; } = string.Empty;
    public string? MotifRefus { get; set; }
    public bool EstValidee { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
