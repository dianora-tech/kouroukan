using GnDapper.Entities;

namespace Pedagogie.Domain.Entities;

/// <summary>
/// Classe (division) rattachee a un niveau pour une annee scolaire.
/// Table : pedagogie.classes
/// </summary>
public class Classe : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom de la classe (ex: "7eme A").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description de la classe.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers le niveau de classe (pedagogie.niveaux_classes).</summary>
    public int NiveauClasseId { get; set; }

    /// <summary>Nombre maximum d'eleves.</summary>
    public int Capacite { get; set; }

    /// <summary>FK vers l'annee scolaire (inscriptions.annees_scolaires).</summary>
    public int AnneeScolaireId { get; set; }

    /// <summary>FK vers l'enseignant principal (personnel.enseignants).</summary>
    public int? EnseignantPrincipalId { get; set; }

    /// <summary>Nombre d'eleves inscrits.</summary>
    public int Effectif { get; set; }

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
