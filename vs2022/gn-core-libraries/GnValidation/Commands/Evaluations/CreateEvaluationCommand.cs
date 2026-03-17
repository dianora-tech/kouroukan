namespace GnValidation.Commands.Evaluations;

/// <summary>Commande de creation d'une evaluation.</summary>
public record CreateEvaluationCommand(
    string Name,
    int MatiereId,
    int ClasseId,
    int EnseignantId,
    DateTime DateEvaluation,
    decimal Coefficient,
    decimal NoteMaximale,
    int Trimestre,
    int AnneeScolaireId);
