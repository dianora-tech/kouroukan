namespace GnDapper.Entities;

/// <summary>
/// Interface pour les entites auditables.
/// Fournit les champs de suivi de creation et de modification.
/// </summary>
public interface IAuditableEntity : IEntity
{
    /// <summary>
    /// Date et heure de creation de l'entite (UTC).
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure de la derniere modification (UTC). Null si jamais modifiee.
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur ayant cree l'entite.
    /// </summary>
    string CreatedBy { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur ayant effectue la derniere modification. Null si jamais modifiee.
    /// </summary>
    string? UpdatedBy { get; set; }
}
