using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les bulletins.
/// </summary>
public sealed record GetAllBulletinsQuery() : IRequest<IReadOnlyList<Bulletin>>;
