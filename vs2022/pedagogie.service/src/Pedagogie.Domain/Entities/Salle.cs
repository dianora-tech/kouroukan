using GnDapper.Entities;

namespace Pedagogie.Domain.Entities;

/// <summary>
/// Salle de cours ou espace pedagogique.
/// Table : pedagogie.salles
/// </summary>
public class Salle : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom de la salle (ex: "Salle 101").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description de la salle.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers le type de salle.</summary>
    public int TypeId { get; set; }

    /// <summary>Capacite de la salle.</summary>
    public int Capacite { get; set; }

    /// <summary>Nom du batiment.</summary>
    public string? Batiment { get; set; }

    /// <summary>Liste des equipements (JSON ou texte libre).</summary>
    public string? Equipements { get; set; }

    /// <summary>Disponibilite de la salle.</summary>
    public bool EstDisponible { get; set; }

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
