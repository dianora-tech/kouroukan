using GnDapper.Entities;

namespace Inscriptions.Domain.Entities;

/// <summary>
/// Annee scolaire (ex: 2025-2026).
/// Table : inscriptions.annees_scolaires
/// </summary>
public class AnneeScolaire : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Libelle de l'annee scolaire (ex: "2025-2026").</summary>
    public string Libelle { get; set; } = string.Empty;

    /// <summary>Date de debut de l'annee scolaire.</summary>
    public DateTime DateDebut { get; set; }

    /// <summary>Date de fin de l'annee scolaire.</summary>
    public DateTime DateFin { get; set; }

    /// <summary>Indique si cette annee scolaire est l'annee en cours.</summary>
    public bool EstActive { get; set; }

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
