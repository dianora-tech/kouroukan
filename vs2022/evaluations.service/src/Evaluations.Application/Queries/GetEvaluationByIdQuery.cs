using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Queries;

/// <summary>
/// Requete de recuperation d'une evaluation par son identifiant.
/// </summary>
public sealed record GetEvaluationByIdQuery(int Id) : IRequest<Evaluation?>;
