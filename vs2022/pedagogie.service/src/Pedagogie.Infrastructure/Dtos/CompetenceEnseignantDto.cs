using GnDapper.Entities;

namespace Pedagogie.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table pedagogie.competences_enseignant.
/// </summary>
public sealed class CompetenceEnseignantDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MatiereId { get; set; }
    public string CycleEtude { get; set; } = string.Empty;

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }

    // ISoftDeletable
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
