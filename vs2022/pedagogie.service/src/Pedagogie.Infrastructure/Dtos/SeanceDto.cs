using GnDapper.Entities;

namespace Pedagogie.Infrastructure.Dtos;

public sealed class SeanceDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MatiereId { get; set; }
    public int ClasseId { get; set; }
    public int EnseignantId { get; set; }
    public int SalleId { get; set; }
    public string JourSemaine { get; set; } = string.Empty;
    public TimeSpan HeureDebut { get; set; }
    public TimeSpan HeureFin { get; set; }
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
