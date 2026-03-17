using GnDapper.Entities;

namespace Bde.Domain.Entities;

/// <summary>
/// Evenement organise par une association.
/// Table : bde.evenements
/// </summary>
public class Evenement : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom de l'evenement.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description de l'evenement.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers le type d'evenement.</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers l'association organisatrice.</summary>
    public int AssociationId { get; set; }

    /// <summary>Date et heure de l'evenement.</summary>
    public DateTime DateEvenement { get; set; }

    /// <summary>Lieu de l'evenement.</summary>
    public string Lieu { get; set; } = string.Empty;

    /// <summary>Capacite maximale de participants.</summary>
    public int? Capacite { get; set; }

    /// <summary>Tarif d'entree (GNF). Null si gratuit.</summary>
    public decimal? TarifEntree { get; set; }

    /// <summary>Nombre d'inscrits a l'evenement.</summary>
    public int NombreInscrits { get; set; }

    /// <summary>Statut de l'evenement : Planifie, Valide, EnCours, Termine, Annule.</summary>
    public string StatutEvenement { get; set; } = string.Empty;

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
