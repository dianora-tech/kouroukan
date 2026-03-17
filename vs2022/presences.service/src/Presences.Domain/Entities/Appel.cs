using GnDapper.Entities;

namespace Presences.Domain.Entities;

/// <summary>
/// Appel effectue par un enseignant pour une classe lors d'une seance.
/// Table : presences.appels
/// </summary>
public class Appel : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers la classe.</summary>
    public int ClasseId { get; set; }

    /// <summary>FK vers l'enseignant qui fait l'appel.</summary>
    public int EnseignantId { get; set; }

    /// <summary>FK vers la seance (optionnel).</summary>
    public int? SeanceId { get; set; }

    /// <summary>Date de l'appel.</summary>
    public DateTime DateAppel { get; set; }

    /// <summary>Heure de l'appel.</summary>
    public TimeSpan HeureAppel { get; set; }

    /// <summary>Indique si l'appel est cloture.</summary>
    public bool EstCloture { get; set; }

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
