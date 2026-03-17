using GnDapper.Entities;

namespace Presences.Domain.Entities;

/// <summary>
/// Absence d'un eleve, liee ou non a un appel.
/// Table : presences.absences
/// </summary>
public class Absence : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers le type d'absence.</summary>
    public int TypeId { get; set; }

    /// <summary>FK vers l'eleve absent.</summary>
    public int EleveId { get; set; }

    /// <summary>FK vers l'appel (optionnel).</summary>
    public int? AppelId { get; set; }

    /// <summary>Date de l'absence.</summary>
    public DateTime DateAbsence { get; set; }

    /// <summary>Heure de debut de l'absence.</summary>
    public TimeSpan? HeureDebut { get; set; }

    /// <summary>Heure de fin de l'absence.</summary>
    public TimeSpan? HeureFin { get; set; }

    /// <summary>Indique si l'absence est justifiee.</summary>
    public bool EstJustifiee { get; set; }

    /// <summary>Motif de la justification.</summary>
    public string? MotifJustification { get; set; }

    /// <summary>URL du justificatif (certificat medical, etc.).</summary>
    public string? PieceJointeUrl { get; set; }

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
