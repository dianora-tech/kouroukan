using GnDapper.Entities;

namespace Pedagogie.Domain.Entities;

/// <summary>
/// Competence d'un enseignant pour une matiere et un cycle d'etude.
/// Table : pedagogie.competences_enseignant
/// </summary>
public class CompetenceEnseignant : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers l'utilisateur enseignant (auth.users).</summary>
    public int UserId { get; set; }

    /// <summary>FK vers la matiere (pedagogie.matieres).</summary>
    public int MatiereId { get; set; }

    /// <summary>Cycle d'etude (ex: "Primaire", "College", "Lycee").</summary>
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
