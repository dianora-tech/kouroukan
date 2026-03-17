using GnDapper.Entities;

namespace Pedagogie.Domain.Entities;

/// <summary>
/// Cahier de textes — contenu du cours et devoirs associes a une seance.
/// Table : pedagogie.cahiers_textes
/// </summary>
public class CahierTextes : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Titre de l'entree du cahier de textes.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description complementaire.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers la seance (pedagogie.seances).</summary>
    public int SeanceId { get; set; }

    /// <summary>Contenu du cours.</summary>
    public string Contenu { get; set; } = string.Empty;

    /// <summary>Date de la seance.</summary>
    public DateTime DateSeance { get; set; }

    /// <summary>Devoirs a faire pour la prochaine seance.</summary>
    public string? TravailAFaire { get; set; }

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
