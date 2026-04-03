using GnDapper.Entities;

namespace Evaluations.Domain.Entities;

/// <summary>
/// Bulletin de notes d'un eleve pour un trimestre.
/// Table : evaluations.bulletins
/// </summary>
public class Bulletin : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>FK vers l'eleve (inscriptions.eleves).</summary>
    public int EleveId { get; set; }

    /// <summary>FK vers la classe (pedagogie.classes).</summary>
    public int ClasseId { get; set; }

    /// <summary>Trimestre concerne (1, 2, 3).</summary>
    public int Trimestre { get; set; }

    /// <summary>FK vers l'annee scolaire (inscriptions.annees_scolaires).</summary>
    public int AnneeScolaireId { get; set; }

    /// <summary>Moyenne generale de l'eleve.</summary>
    public decimal MoyenneGenerale { get; set; }

    /// <summary>Rang de l'eleve dans la classe.</summary>
    public int? Rang { get; set; }

    /// <summary>Appreciation du conseil de classe.</summary>
    public string? Appreciation { get; set; }

    /// <summary>Bulletin publie aux parents.</summary>
    public bool EstPublie { get; set; }

    /// <summary>Date de generation du bulletin.</summary>
    public DateTime DateGeneration { get; set; }

    /// <summary>Chemin du fichier PDF genere.</summary>
    public string? CheminFichierPdf { get; set; }

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
