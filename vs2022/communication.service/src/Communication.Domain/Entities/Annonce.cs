using GnDapper.Entities;

namespace Communication.Domain.Entities;

/// <summary>
/// Annonce publiee sur la plateforme pour un public cible.
/// Table : communication.annonces
/// </summary>
public class Annonce : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Titre de l'annonce.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>FK vers le type d'annonce (communication.type_annonces).</summary>
    public int TypeId { get; set; }

    /// <summary>Contenu de l'annonce.</summary>
    public string Contenu { get; set; } = string.Empty;

    /// <summary>Date de debut d'affichage.</summary>
    public DateTime DateDebut { get; set; }

    /// <summary>Date de fin d'affichage.</summary>
    public DateTime? DateFin { get; set; }

    /// <summary>Annonce active.</summary>
    public bool EstActive { get; set; }

    /// <summary>Cible : Tous/Parents/Enseignants/Eleves.</summary>
    public string CibleAudience { get; set; } = string.Empty;

    /// <summary>Priorite d'affichage (1=haute).</summary>
    public int Priorite { get; set; }

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
