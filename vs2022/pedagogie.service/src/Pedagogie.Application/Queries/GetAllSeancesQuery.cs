using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les seances.
/// </summary>
public sealed record GetAllSeancesQuery() : IRequest<IReadOnlyList<Seance>>;
