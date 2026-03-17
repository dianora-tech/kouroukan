namespace GnValidation.Commands.Pedagogie;

/// <summary>Commande de creation d'une seance.</summary>
public record CreateSeanceCommand(
    int MatiereId,
    int ClasseId,
    int EnseignantId,
    int SalleId,
    int JourSemaine,
    TimeSpan HeureDebut,
    TimeSpan HeureFin,
    int AnneeScolaireId);
