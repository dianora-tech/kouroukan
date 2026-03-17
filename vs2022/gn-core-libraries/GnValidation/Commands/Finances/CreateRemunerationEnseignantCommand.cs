namespace GnValidation.Commands.Finances;

/// <summary>Commande de creation d'une remuneration enseignant.</summary>
public record CreateRemunerationEnseignantCommand(
    int EnseignantId,
    int Mois,
    int Annee,
    string ModeRemuneration,
    decimal? MontantForfait,
    decimal? NombreHeures,
    decimal? TauxHoraire,
    decimal MontantTotal,
    string StatutPaiement);
