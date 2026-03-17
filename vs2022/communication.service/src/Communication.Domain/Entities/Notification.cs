using GnDapper.Entities;

namespace Communication.Domain.Entities;

/// <summary>
/// Notification push/SMS/email/in-app envoyee aux utilisateurs.
/// Table : communication.notifications
/// </summary>
public class Notification : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom de la notification.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description de la notification.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers le type de notification (communication.type_notifications).</summary>
    public int TypeId { get; set; }

    /// <summary>IDs des destinataires au format JSON.</summary>
    public string DestinatairesIds { get; set; } = string.Empty;

    /// <summary>Contenu de la notification.</summary>
    public string Contenu { get; set; } = string.Empty;

    /// <summary>Canal d'envoi (Push/SMS/Email/InApp).</summary>
    public string Canal { get; set; } = string.Empty;

    /// <summary>Notification envoyee.</summary>
    public bool EstEnvoyee { get; set; }

    /// <summary>Date d'envoi de la notification.</summary>
    public DateTime? DateEnvoi { get; set; }

    /// <summary>Lien deep link action.</summary>
    public string? LienAction { get; set; }

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
