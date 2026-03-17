using GnDapper.Entities;

namespace Bde.Domain.Entities;

/// <summary>
/// Association etudiante (BDE, club sportif, culturel, etc.).
/// Table : bde.associations
/// </summary>
public class Association : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom de l'association.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description de l'association.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers le type d'association.</summary>
    public int TypeId { get; set; }

    /// <summary>Sigle de l'association (ex: BDE, AS).</summary>
    public string? Sigle { get; set; }

    /// <summary>Annee scolaire (ex: "2025-2026").</summary>
    public string AnneeScolaire { get; set; } = string.Empty;

    /// <summary>Statut de l'association : Active, Suspendue, Dissoute.</summary>
    public string Statut { get; set; } = string.Empty;

    /// <summary>Budget annuel alloue (GNF).</summary>
    public decimal BudgetAnnuel { get; set; }

    /// <summary>FK vers le superviseur (personnel de l'etablissement).</summary>
    public int? SuperviseurId { get; set; }

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
