using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les evaluations.
/// </summary>
public sealed record GetAllEvaluationsQuery() : IRequest<IReadOnlyList<Evaluation>>;
