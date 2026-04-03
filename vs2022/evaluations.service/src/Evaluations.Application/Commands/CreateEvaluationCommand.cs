using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Commands;

/// <summary>
/// Commande de creation d'une evaluation.
/// </summary>
public sealed record CreateEvaluationCommand(
    int TypeId,
    int MatiereId,
    int ClasseId,
    int EnseignantId,
    DateTime DateEvaluation,
    decimal Coefficient,
    decimal NoteMaximale,
    int Trimestre,
    int AnneeScolaireId,
    int UserId) : IRequest<Evaluation>;
