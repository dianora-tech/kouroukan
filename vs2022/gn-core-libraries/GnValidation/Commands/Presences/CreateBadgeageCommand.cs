namespace GnValidation.Commands.Presences;

/// <summary>Commande de creation d'un badgeage.</summary>
public record CreateBadgeageCommand(
    int EleveId,
    DateTime DateBadgeage,
    TimeSpan HeureBadgeage,
    string PointAcces,
    string MethodeBadgeage);
