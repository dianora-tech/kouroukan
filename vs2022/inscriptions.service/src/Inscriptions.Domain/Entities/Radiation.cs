using GnDapper.Entities;

namespace Inscriptions.Domain.Entities;

/// <summary>
/// Radiation d'un eleve d'un etablissement.
/// Table : inscriptions.radiations
/// </summary>
public class Radiation : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers l'eleve (inscriptions.eleves).</summary>
    public int EleveId { get; set; }

    /// <summary>FK vers l'etablissement (auth.companies).</summary>
    public int CompanyId { get; set; }

    /// <summary>Motif de la radiation.</summary>
    public string Motif { get; set; } = string.Empty;

    /// <summary>Documents associes a la radiation (JSON).</summary>
    public string? Documents { get; set; }

    /// <summary>Date de la radiation.</summary>
    public DateTime DateRadiation { get; set; }

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
