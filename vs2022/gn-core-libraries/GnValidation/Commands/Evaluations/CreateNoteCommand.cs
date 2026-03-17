namespace GnValidation.Commands.Evaluations;

/// <summary>Commande de creation d'une note.</summary>
public record CreateNoteCommand(
    int EvaluationId,
    int EleveId,
    decimal Valeur,
    string? Commentaire);
