namespace GnDapper.Entities;

/// <summary>
/// Interface pour les entites supportant la suppression logique.
/// Les entites ne sont pas physiquement supprimees mais marquees comme supprimees.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Indique si l'entite est logiquement supprimee.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Date et heure de la suppression logique (UTC). Null si non supprimee.
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur ayant effectue la suppression logique. Null si non supprimee.
    /// </summary>
    string? DeletedBy { get; set; }
}
