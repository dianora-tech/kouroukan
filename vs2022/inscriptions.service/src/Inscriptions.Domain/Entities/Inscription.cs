using GnDapper.Entities;

namespace Inscriptions.Domain.Entities;

/// <summary>
/// Inscription d'un eleve dans une classe pour une annee scolaire.
/// Table : inscriptions.inscriptions
/// </summary>
public class Inscription : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers le type d'inscription.</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers l'eleve.</summary>
    public int EleveId { get; set; }

    /// <summary>FK vers la classe.</summary>
    public int ClasseId { get; set; }

    /// <summary>FK vers l'annee scolaire.</summary>
    public int AnneeScolaireId { get; set; }

    /// <summary>Date d'inscription.</summary>
    public DateTime DateInscription { get; set; }

    /// <summary>Montant des frais d'inscription (GNF).</summary>
    public decimal MontantInscription { get; set; }

    /// <summary>Paiement effectue ou non.</summary>
    public bool EstPaye { get; set; }

    /// <summary>Eleve redoublant cette annee (enjeu majeur systeme guineen).</summary>
    public bool EstRedoublant { get; set; }

    /// <summary>Type d'etablissement : Public, PriveLaique, PriveFrancoArabe, Communautaire, PriveCatholique, PriveProtestant.</summary>
    public string? TypeEtablissement { get; set; }

    /// <summary>Serie du baccalaureat (SE, SM, SS, FA) — uniquement pour les eleves du lycee.</summary>
    public string? SerieBac { get; set; }

    /// <summary>Statut de l'inscription : EnAttente, Validee, Annulee.</summary>
    public string StatutInscription { get; set; } = string.Empty;

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
