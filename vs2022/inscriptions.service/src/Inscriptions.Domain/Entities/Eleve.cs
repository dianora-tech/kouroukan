using GnDapper.Entities;

namespace Inscriptions.Domain.Entities;

/// <summary>
/// Eleve inscrit dans un etablissement scolaire.
/// Table : inscriptions.eleves
/// </summary>
public class Eleve : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Prenom de l'eleve.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Nom de l'eleve.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Date de naissance.</summary>
    public DateTime DateNaissance { get; set; }

    /// <summary>Lieu de naissance.</summary>
    public string LieuNaissance { get; set; } = string.Empty;

    /// <summary>Genre (M ou F).</summary>
    public string Genre { get; set; } = string.Empty;

    /// <summary>Nationalite.</summary>
    public string Nationalite { get; set; } = string.Empty;

    /// <summary>Adresse du domicile.</summary>
    public string? Adresse { get; set; }

    /// <summary>URL de la photo.</summary>
    public string? PhotoUrl { get; set; }

    /// <summary>Matricule unique de l'eleve.</summary>
    public string NumeroMatricule { get; set; } = string.Empty;

    /// <summary>FK vers le niveau de classe (pedagogie.niveaux_classes).</summary>
    public int NiveauClasseId { get; set; }

    /// <summary>FK vers la classe affectee (pedagogie.classes).</summary>
    public int? ClasseId { get; set; }

    /// <summary>FK vers le parent/tuteur (auth.users).</summary>
    public int? ParentId { get; set; }

    /// <summary>Statut d'inscription : Prospect, PreInscrit, Inscrit, Radie.</summary>
    public string StatutInscription { get; set; } = string.Empty;

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
