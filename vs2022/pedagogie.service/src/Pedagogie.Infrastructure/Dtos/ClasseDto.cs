using GnDapper.Entities;

namespace Pedagogie.Infrastructure.Dtos;

public sealed class ClasseDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int NiveauClasseId { get; set; }
    public int Capacite { get; set; }
    public int AnneeScolaireId { get; set; }
    public int? EnseignantPrincipalId { get; set; }
    public int Effectif { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
