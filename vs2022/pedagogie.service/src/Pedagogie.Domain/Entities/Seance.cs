using GnDapper.Entities;

namespace Pedagogie.Domain.Entities;

/// <summary>
/// Seance de cours dans l'emploi du temps.
/// Table : pedagogie.seances
/// </summary>
public class Seance : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom de la seance.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description de la seance.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers la matiere (pedagogie.matieres).</summary>
    public int MatiereId { get; set; }

    /// <summary>FK vers la classe (pedagogie.classes).</summary>
    public int ClasseId { get; set; }

    /// <summary>FK vers l'enseignant (personnel.enseignants).</summary>
    public int EnseignantId { get; set; }

    /// <summary>FK vers la salle (pedagogie.salles).</summary>
    public int SalleId { get; set; }

    /// <summary>Jour de la semaine (1=Lundi ... 6=Samedi).</summary>
    public int JourSemaine { get; set; }

    /// <summary>Heure de debut de la seance.</summary>
    public TimeSpan HeureDebut { get; set; }

    /// <summary>Heure de fin de la seance.</summary>
    public TimeSpan HeureFin { get; set; }

    /// <summary>FK vers l'annee scolaire (inscriptions.annees_scolaires).</summary>
    public int AnneeScolaireId { get; set; }

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
