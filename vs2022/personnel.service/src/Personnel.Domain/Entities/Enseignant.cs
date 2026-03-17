using GnDapper.Entities;

namespace Personnel.Domain.Entities;

/// <summary>
/// Enseignant d'un etablissement scolaire.
/// Table : personnel.enseignants
/// </summary>
public class Enseignant : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom affiché (hérité de IEntity).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description optionnelle.</summary>
    public string? Description { get; set; }

    /// <summary>Matricule unique de l'enseignant.</summary>
    public string Matricule { get; set; } = string.Empty;

    /// <summary>Specialite(s) enseignee(s).</summary>
    public string Specialite { get; set; } = string.Empty;

    /// <summary>Date d'embauche.</summary>
    public DateTime DateEmbauche { get; set; }

    /// <summary>Mode de remuneration : Forfait, Heures ou Mixte.</summary>
    public string ModeRemuneration { get; set; } = string.Empty;

    /// <summary>Forfait mensuel si applicable.</summary>
    public decimal? MontantForfait { get; set; }

    /// <summary>Telephone (+224 xxx).</summary>
    public string Telephone { get; set; } = string.Empty;

    /// <summary>Email de l'enseignant.</summary>
    public string? Email { get; set; }

    /// <summary>Statut : Actif, EnConge, Suspendu, Inactif.</summary>
    public string StatutEnseignant { get; set; } = string.Empty;

    /// <summary>Solde de conges restant.</summary>
    public int SoldeCongesAnnuel { get; set; }

    /// <summary>FK vers le type d'enseignant (personnel.types).</summary>
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
