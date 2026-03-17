using GnDapper.Entities;

namespace Documents.Domain.Entities;

/// <summary>
/// Document genere a partir d'un modele.
/// Table : documents.documents_generes
/// </summary>
public class DocumentGenere : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers le type de document genere.</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers le modele de document utilise.</summary>
    public int ModeleDocumentId { get; set; }

    /// <summary>FK vers l'eleve concerne (nullable).</summary>
    public int? EleveId { get; set; }

    /// <summary>FK vers l'enseignant concerne (nullable).</summary>
    public int? EnseignantId { get; set; }

    /// <summary>Donnees de merge au format JSON.</summary>
    public string DonneesJson { get; set; } = string.Empty;

    /// <summary>Date de generation du document.</summary>
    public DateTime DateGeneration { get; set; }

    /// <summary>Statut de la signature : EnAttente, EnCours, Signe, Refuse.</summary>
    public string StatutSignature { get; set; } = string.Empty;

    /// <summary>Chemin du fichier PDF genere.</summary>
    public string? CheminFichier { get; set; }

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
