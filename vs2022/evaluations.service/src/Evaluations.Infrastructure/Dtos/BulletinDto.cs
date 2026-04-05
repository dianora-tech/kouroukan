using GnDapper.Attributes;
using GnDapper.Entities;

namespace Evaluations.Infrastructure.Dtos;

[Table("evaluations.bulletins")]
public sealed class BulletinDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int EleveId { get; set; }
    public int ClasseId { get; set; }
    public int Trimestre { get; set; }
    public int AnneeScolaireId { get; set; }
    public decimal MoyenneGenerale { get; set; }
    public int? Rang { get; set; }
    public string? Appreciation { get; set; }
    public bool EstPublie { get; set; }
    public DateTime DateGeneration { get; set; }
    public string? CheminFichierPdf { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
