using GnDapper.Attributes;
using GnDapper.Entities;

namespace Evaluations.Infrastructure.Dtos;

[Table("evaluations.evaluations")]
public sealed class EvaluationDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public int MatiereId { get; set; }
    public int ClasseId { get; set; }
    public int EnseignantId { get; set; }
    public DateTime DateEvaluation { get; set; }
    public decimal Coefficient { get; set; }
    public decimal NoteMaximale { get; set; }
    public int Trimestre { get; set; }
    public int AnneeScolaireId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
