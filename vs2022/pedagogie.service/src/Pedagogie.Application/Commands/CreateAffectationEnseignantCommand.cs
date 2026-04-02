using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de creation d'une affectation enseignant.
/// </summary>
public sealed record CreateAffectationEnseignantCommand(
    int LiaisonId,
    int ClasseId,
    int MatiereId,
    int AnneeScolaireId) : IRequest<AffectationEnseignant>;
