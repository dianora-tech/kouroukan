namespace GnValidation.Commands.Presences;

/// <summary>Commande de creation d'un appel.</summary>
public record CreateAppelCommand(
    int ClasseId,
    int EnseignantId,
    int? SeanceId,
    DateTime DateAppel,
    TimeSpan HeureAppel,
    bool EstCloture);
