using GnDapper.Entities;

namespace Pedagogie.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table pedagogie.affectations_enseignant.
/// </summary>
public sealed class AffectationEnseignantDto : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public int LiaisonId { get; set; }
    public int ClasseId { get; set; }
    public int MatiereId { get; set; }
    public int AnneeScolaireId { get; set; }
    public bool EstActive { get; set; }

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
