using GnDapper.Entities;

namespace Presences.Domain.Entities;

/// <summary>
/// Badgeage NFC/QR Code d'un eleve a un point d'acces.
/// Table : presences.badgeages
/// </summary>
public class Badgeage : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers le type de badgeage.</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers l'eleve.</summary>
    public int EleveId { get; set; }

    /// <summary>Date du badgeage.</summary>
    public DateTime DateBadgeage { get; set; }

    /// <summary>Heure du badgeage.</summary>
    public TimeSpan HeureBadgeage { get; set; }

    /// <summary>Point d'acces : Entree/Sortie/Cantine/Biblio.</summary>
    public string PointAcces { get; set; } = string.Empty;

    /// <summary>Methode de badgeage : NFC/QRCode/Manuel.</summary>
    public string MethodeBadgeage { get; set; } = string.Empty;

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
