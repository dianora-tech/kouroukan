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

    /// <summary>Code de l'annee scolaire (ex: "2024-2025").</summary>
    public string? Code { get; set; }

    /// <summary>Description de l'annee scolaire.</summary>
    public string? Description { get; set; }

    /// <summary>Statut de l'annee scolaire (preparation, active, cloturee, archivee).</summary>
    public string Statut { get; set; } = "preparation";

    /// <summary>Date effective de rentree.</summary>
    public DateTime? DateRentree { get; set; }

    /// <summary>Nombre de periodes (trimestres ou semestres).</summary>
    public int NombrePeriodes { get; set; } = 3;

    /// <summary>Type de periode (trimestre ou semestre).</summary>
    public string TypePeriode { get; set; } = "trimestre";

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
