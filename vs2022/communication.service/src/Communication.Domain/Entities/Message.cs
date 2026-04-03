using GnDapper.Entities;

namespace Communication.Domain.Entities;

/// <summary>
/// Message interne entre utilisateurs de la plateforme.
/// Table : communication.messages
/// </summary>
public class Message : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers le type de message (communication.type_messages).</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers l'expediteur (auth.users).</summary>
    public int ExpediteurId { get; set; }

    /// <summary>FK vers le destinataire individuel (auth.users).</summary>
    public int? DestinataireId { get; set; }

    /// <summary>Sujet du message.</summary>
    public string Sujet { get; set; } = string.Empty;

    /// <summary>Contenu du message.</summary>
    public string Contenu { get; set; } = string.Empty;

    /// <summary>Message lu par le destinataire.</summary>
    public bool EstLu { get; set; }

    /// <summary>Date de lecture du message.</summary>
    public DateTime? DateLecture { get; set; }

    /// <summary>Groupe cible (classe, niveau, role).</summary>
    public string? GroupeDestinataire { get; set; }

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
