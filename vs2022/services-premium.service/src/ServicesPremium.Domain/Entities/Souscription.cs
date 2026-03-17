using GnDapper.Entities;
using GnDapper.Models;

namespace ServicesPremium.Domain.Entities;

/// <summary>
/// Souscription d'un parent a un service premium.
/// </summary>
public class Souscription : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>FK vers ServiceParent.</summary>
    public int ServiceParentId { get; set; }

    /// <summary>FK vers Parent.</summary>
    public int ParentId { get; set; }

    /// <summary>Date debut souscription.</summary>
    public DateTime DateDebut { get; set; }

    /// <summary>Date fin souscription.</summary>
    public DateTime? DateFin { get; set; }

    /// <summary>Statut : Active, Expiree, Resiliee, Essai.</summary>
    public string StatutSouscription { get; set; } = string.Empty;

    /// <summary>Montant paye.</summary>
    public decimal MontantPaye { get; set; }

    /// <summary>Renouvellement automatique.</summary>
    public bool RenouvellementAuto { get; set; }

    /// <summary>Date du prochain renouvellement.</summary>
    public DateTime? DateProchainRenouvellement { get; set; }

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
