using MediatR;

namespace Evaluations.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une evaluation.
/// </summary>
public sealed record UpdateEvaluationCommand(
    int Id,
    int TypeId,
    int MatiereId,
    int ClasseId,
    int EnseignantId,
    DateTime DateEvaluation,
    decimal Coefficient,
    decimal NoteMaximale,
    int Trimestre,
    int AnneeScolaireId,
    int UserId) : IRequest<bool>;
