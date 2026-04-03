using GnDapper.Entities;

namespace Evaluations.Domain.Entities;

/// <summary>
/// Evaluation (devoir, examen, interrogation, etc.) dans une matiere pour une classe.
/// Table : evaluations.evaluations
/// </summary>
public class Evaluation : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers le type d'evaluation (evaluations.type_evaluations).</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers la matiere (pedagogie.matieres).</summary>
    public int MatiereId { get; set; }

    /// <summary>FK vers la classe (pedagogie.classes).</summary>
    public int ClasseId { get; set; }

    /// <summary>FK vers l'enseignant (personnel.enseignants).</summary>
    public int EnseignantId { get; set; }

    /// <summary>Date de l'evaluation.</summary>
    public DateTime DateEvaluation { get; set; }

    /// <summary>Coefficient de l'evaluation.</summary>
    public decimal Coefficient { get; set; }

    /// <summary>Note maximale (20, 10, etc.).</summary>
    public decimal NoteMaximale { get; set; }

    /// <summary>Trimestre (1, 2, 3) ou Semestre.</summary>
    public int Trimestre { get; set; }

    /// <summary>FK vers l'annee scolaire (inscriptions.annees_scolaires).</summary>
    public int AnneeScolaireId { get; set; }

    /// <summary>FK vers l'utilisateur (auth.users).</summary>
    public int UserId { get; set; }

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
