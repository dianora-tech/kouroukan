using GnDapper.Entities;

namespace Documents.Domain.Entities;

/// <summary>
/// Signature electronique sur un document genere.
/// Table : documents.signatures
/// </summary>
public class Signature : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers le type de signature.</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers le document genere.</summary>
    public int DocumentGenereId { get; set; }

    /// <summary>FK vers le signataire (auth.users).</summary>
    public int SignataireId { get; set; }

    /// <summary>Ordre dans la chaine de signature.</summary>
    public int OrdreSignature { get; set; }

    /// <summary>Date de la signature (nullable si pas encore signe).</summary>
    public DateTime? DateSignature { get; set; }

    /// <summary>Statut de la signature : EnAttente, Signe, Refuse, Delegue.</summary>
    public string StatutSignature { get; set; } = string.Empty;

    /// <summary>Niveau de signature : Basique, Avancee.</summary>
    public string NiveauSignature { get; set; } = string.Empty;

    /// <summary>Motif en cas de refus.</summary>
    public string? MotifRefus { get; set; }

    /// <summary>Signature validee ou non.</summary>
    public bool EstValidee { get; set; }

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
