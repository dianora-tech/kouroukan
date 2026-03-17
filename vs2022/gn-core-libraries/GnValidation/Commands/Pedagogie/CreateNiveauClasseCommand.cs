namespace GnValidation.Commands.Pedagogie;

/// <summary>Commande de creation d'un niveau de classe.</summary>
public record CreateNiveauClasseCommand(
    string Name,
    string Code,
    int Ordre,
    string CycleEtude,
    int? AgeOfficielEntree,
    string? MinistereTutelle,
    string? ExamenSortie,
    decimal? TauxHoraireEnseignant);
