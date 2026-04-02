using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de suppression d'une competence enseignant.
/// </summary>
public sealed record DeleteCompetenceEnseignantCommand(int Id) : IRequest<bool>;
