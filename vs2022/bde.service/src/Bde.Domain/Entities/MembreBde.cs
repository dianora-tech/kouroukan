using GnDapper.Entities;

namespace Bde.Domain.Entities;

/// <summary>
/// Membre d'une association BDE.
/// Table : bde.membres_bde
/// </summary>
public class MembreBde : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom du membre (affichage).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description ou notes sur le membre.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers l'association.</summary>
    public int AssociationId { get; set; }

    /// <summary>FK vers l'eleve.</summary>
    public int EleveId { get; set; }

    /// <summary>Role au sein du BDE : President, Tresorier, Secretaire, RespPole, Membre.</summary>
    public string RoleBde { get; set; } = string.Empty;

    /// <summary>Date d'adhesion a l'association.</summary>
    public DateTime DateAdhesion { get; set; }

    /// <summary>Montant de la cotisation payee (GNF).</summary>
    public decimal? MontantCotisation { get; set; }

    /// <summary>Indique si le membre est actif.</summary>
    public bool EstActif { get; set; }

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
