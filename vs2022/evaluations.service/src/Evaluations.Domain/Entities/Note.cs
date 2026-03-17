using GnDapper.Entities;

namespace Evaluations.Domain.Entities;

/// <summary>
/// Note obtenue par un eleve a une evaluation.
/// Table : evaluations.notes
/// </summary>
public class Note : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom (libelle) de la note.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description complementaire.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers l'evaluation (evaluations.evaluations).</summary>
    public int EvaluationId { get; set; }

    /// <summary>FK vers l'eleve (inscriptions.eleves).</summary>
    public int EleveId { get; set; }

    /// <summary>Note obtenue.</summary>
    public decimal Valeur { get; set; }

    /// <summary>Appreciation de l'enseignant.</summary>
    public string? Commentaire { get; set; }

    /// <summary>Date de saisie de la note.</summary>
    public DateTime DateSaisie { get; set; }

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
