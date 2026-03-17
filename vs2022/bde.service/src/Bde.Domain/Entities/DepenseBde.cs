using GnDapper.Entities;

namespace Bde.Domain.Entities;

/// <summary>
/// Depense effectuee par une association BDE.
/// Table : bde.depenses_bde
/// </summary>
public class DepenseBde : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Libelle de la depense.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description detaillee de la depense.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers le type de depense BDE.</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers l'association.</summary>
    public int AssociationId { get; set; }

    /// <summary>Montant de la depense (GNF).</summary>
    public decimal Montant { get; set; }

    /// <summary>Motif de la depense.</summary>
    public string Motif { get; set; } = string.Empty;

    /// <summary>Categorie : Materiel, Location, Prestataire, Remboursement.</summary>
    public string Categorie { get; set; } = string.Empty;

    /// <summary>Statut de validation : Demandee, ValideTresorier, ValideSuper, Refusee.</summary>
    public string StatutValidation { get; set; } = string.Empty;

    /// <summary>FK vers le validateur.</summary>
    public int? ValidateurId { get; set; }

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
