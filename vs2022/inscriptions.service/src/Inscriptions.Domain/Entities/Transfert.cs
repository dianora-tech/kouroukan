using GnDapper.Entities;

namespace Inscriptions.Domain.Entities;

/// <summary>
/// Transfert d'un eleve entre deux etablissements.
/// Table : inscriptions.transferts
/// </summary>
public class Transfert : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers l'eleve (inscriptions.eleves).</summary>
    public int EleveId { get; set; }

    /// <summary>FK vers l'etablissement d'origine (auth.companies).</summary>
    public int CompanyOrigineId { get; set; }

    /// <summary>FK vers l'etablissement cible (auth.companies).</summary>
    public int CompanyCibleId { get; set; }

    /// <summary>Statut du transfert : EnAttente, Accepte, Refuse, Complete.</summary>
    public string Statut { get; set; } = string.Empty;

    /// <summary>Motif du transfert.</summary>
    public string Motif { get; set; } = string.Empty;

    /// <summary>Documents associes au transfert (JSON).</summary>
    public string? Documents { get; set; }

    /// <summary>Date de la demande de transfert.</summary>
    public DateTime DateDemande { get; set; }

    /// <summary>Date de traitement du transfert.</summary>
    public DateTime? DateTraitement { get; set; }

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
