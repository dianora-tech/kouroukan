using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Evaluations.Infrastructure.Dtos;

[Table("evaluations.notes")]
public sealed class NoteDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int EvaluationId { get; set; }
    public int EleveId { get; set; }
    public decimal Valeur { get; set; }
    public string? Commentaire { get; set; }
    public DateTime DateSaisie { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
