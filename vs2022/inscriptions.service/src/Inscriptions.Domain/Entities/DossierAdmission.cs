using GnDapper.Entities;

namespace Inscriptions.Domain.Entities;

/// <summary>
/// Dossier d'admission d'un candidat.
/// Table : inscriptions.dossiers_admission
/// </summary>
public class DossierAdmission : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers le type de dossier d'admission.</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers l'eleve candidat.</summary>
    public int EleveId { get; set; }

    /// <summary>FK vers l'annee scolaire.</summary>
    public int AnneeScolaireId { get; set; }

    /// <summary>Statut du dossier : Prospect, PreInscrit, EnEtude, Convoque, Admis, Refuse, ListeAttente.</summary>
    public string StatutDossier { get; set; } = string.Empty;

    /// <summary>Etape courante du workflow d'admission.</summary>
    public string EtapeActuelle { get; set; } = string.Empty;

    /// <summary>Date de la demande initiale.</summary>
    public DateTime DateDemande { get; set; }

    /// <summary>Date de la decision finale.</summary>
    public DateTime? DateDecision { get; set; }

    /// <summary>Motif en cas de refus.</summary>
    public string? MotifRefus { get; set; }

    /// <summary>Score interne du dossier.</summary>
    public decimal? ScoringInterne { get; set; }

    /// <summary>Notes internes sur le dossier.</summary>
    public string? Commentaires { get; set; }

    /// <summary>FK vers le responsable des admissions (auth.users).</summary>
    public int? ResponsableAdmissionId { get; set; }

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
