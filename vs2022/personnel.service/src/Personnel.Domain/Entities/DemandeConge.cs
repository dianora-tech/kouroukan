using GnDapper.Entities;

namespace Personnel.Domain.Entities;

/// <summary>
/// Demande de conge d'un enseignant.
/// Table : personnel.demandes_conges
/// </summary>
public class DemandeConge : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom affiché (hérité de IEntity).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description optionnelle.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers l'enseignant demandeur.</summary>
    public int EnseignantId { get; set; }

    /// <summary>Date de debut du conge.</summary>
    public DateTime DateDebut { get; set; }

    /// <summary>Date de fin du conge.</summary>
    public DateTime DateFin { get; set; }

    /// <summary>Motif de la demande.</summary>
    public string Motif { get; set; } = string.Empty;

    /// <summary>Statut : Soumise, ApprouveeN1, ApprouveeDirection, Refusee.</summary>
    public string StatutDemande { get; set; } = string.Empty;

    /// <summary>URL du certificat medical ou autre piece jointe.</summary>
    public string? PieceJointeUrl { get; set; }

    /// <summary>Commentaire du validateur.</summary>
    public string? CommentaireValidateur { get; set; }

    /// <summary>FK vers le validateur.</summary>
    public int? ValidateurId { get; set; }

    /// <summary>Date de la validation ou du refus.</summary>
    public DateTime? DateValidation { get; set; }

    /// <summary>Impact sur la remuneration.</summary>
    public bool ImpactPaie { get; set; }

    /// <summary>FK vers le type de demande de conge (personnel.types).</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers l'utilisateur ayant cree l'enregistrement (auth.users).</summary>
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
