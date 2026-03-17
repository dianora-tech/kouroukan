using MediatR;
using BadgeageEntity = Presences.Domain.Entities.Badgeage;

namespace Presences.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les badgeages.
/// </summary>
public sealed record GetAllBadgeagesQuery() : IRequest<IReadOnlyList<BadgeageEntity>>;
