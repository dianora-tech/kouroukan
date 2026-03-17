using GnDapper.Entities;

namespace Pedagogie.Domain.Entities;

/// <summary>
/// Niveau de classe dans le systeme educatif guineen.
/// Table : pedagogie.niveaux_classes
/// </summary>
public class NiveauClasse : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    /// <summary>Nom complet du niveau (ex: "7eme annee").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Description du niveau.</summary>
    public string? Description { get; set; }

    /// <summary>FK vers le type de niveau.</summary>
    public int TypeId { get; set; }

    /// <summary>Code guineen du niveau (CP1, CP2, CE1, CE2, CM1, CM2, 7E, 8E, 9E, 10E, 11E, 12E, TLE, L1...).</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Ordre de tri (1=PS ... 21=M2).</summary>
    public int Ordre { get; set; }

    /// <summary>Cycle d'etude : Prescolaire, Primaire, College, Lycee, ETFP_PostPrimaire, ETFP_TypeA, ETFP_TypeB, ENF, Universite.</summary>
    public string CycleEtude { get; set; } = string.Empty;

    /// <summary>Age officiel d'entree dans ce niveau (source ProDEG).</summary>
    public int? AgeOfficielEntree { get; set; }

    /// <summary>Ministere de tutelle : MENA, METFP-ET, MESRS.</summary>
    public string? MinistereTutelle { get; set; }

    /// <summary>Examen de sortie du cycle : CEE, BEPC, BU, CQP, BEP, CAP, BTS, Licence, Master, Doctorat.</summary>
    public string? ExamenSortie { get; set; }

    /// <summary>Taux horaire par defaut pour ce niveau (GNF).</summary>
    public decimal? TauxHoraireEnseignant { get; set; }

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
