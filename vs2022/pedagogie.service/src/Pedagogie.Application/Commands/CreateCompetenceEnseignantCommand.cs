using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de creation d'une competence enseignant.
/// </summary>
public sealed record CreateCompetenceEnseignantCommand(
    int UserId,
    int MatiereId,
    string CycleEtude) : IRequest<CompetenceEnseignant>;
