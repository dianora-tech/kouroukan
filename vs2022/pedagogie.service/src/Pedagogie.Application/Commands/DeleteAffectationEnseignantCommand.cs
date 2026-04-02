using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de suppression logique d'une affectation enseignant.
/// </summary>
public sealed record DeleteAffectationEnseignantCommand(int Id) : IRequest<bool>;
