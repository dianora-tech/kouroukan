using MediatR;

namespace Evaluations.Application.Commands;

/// <summary>
/// Commande de suppression d'une evaluation.
/// </summary>
public sealed record DeleteEvaluationCommand(int Id) : IRequest<bool>;
