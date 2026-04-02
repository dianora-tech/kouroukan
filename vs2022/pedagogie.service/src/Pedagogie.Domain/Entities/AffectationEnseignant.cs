using GnDapper.Entities;

namespace Pedagogie.Domain.Entities;

/// <summary>
/// Affectation d'un enseignant a une classe pour une matiere et une annee scolaire.
/// Table : pedagogie.affectations_enseignant
/// </summary>
public class AffectationEnseignant : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers la liaison enseignant-etablissement.</summary>
    public int LiaisonId { get; set; }

    /// <summary>FK vers la classe (pedagogie.classes).</summary>
    public int ClasseId { get; set; }

    /// <summary>FK vers la matiere (pedagogie.matieres).</summary>
    public int MatiereId { get; set; }

    /// <summary>FK vers l'annee scolaire (inscriptions.annees_scolaires).</summary>
    public int AnneeScolaireId { get; set; }

    /// <summary>Indique si l'affectation est active.</summary>
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
