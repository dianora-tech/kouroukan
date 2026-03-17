namespace GnValidation.Commands.Presences;

/// <summary>Commande de creation d'une absence.</summary>
public record CreateAbsenceCommand(
    int EleveId,
    int? AppelId,
    DateTime DateAbsence,
    TimeSpan? HeureDebut,
    TimeSpan? HeureFin,
    bool EstJustifiee,
    string? MotifJustification,
    string? PieceJointeUrl);
