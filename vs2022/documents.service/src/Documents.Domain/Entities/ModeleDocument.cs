using GnDapper.Entities;

namespace Documents.Domain.Entities;

/// <summary>
/// Modele de document institutionnel (template).
/// Table : documents.modeles_documents
/// </summary>
public class ModeleDocument : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers le type de modele de document.</summary>
    public int TypeId { get; set; }

    /// <summary>Code unique du modele (ex: BULL_NOTES, ATT_SCOL).</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Template HTML/Mustache du document.</summary>
    public string Contenu { get; set; } = string.Empty;

    /// <summary>URL du logo de l'etablissement.</summary>
    public string? LogoUrl { get; set; }

    /// <summary>Couleur primaire hex (ex: #16a34a).</summary>
    public string? CouleurPrimaire { get; set; }

    /// <summary>Texte du pied de page.</summary>
    public string? TextePiedPage { get; set; }

    /// <summary>Modele actif ou non.</summary>
    public bool EstActif { get; set; }

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
