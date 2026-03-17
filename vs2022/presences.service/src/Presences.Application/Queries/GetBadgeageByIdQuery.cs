using MediatR;
using BadgeageEntity = Presences.Domain.Entities.Badgeage;

namespace Presences.Application.Queries;

/// <summary>
/// Requete de recuperation d'un badgeage par son identifiant.
/// </summary>
public sealed record GetBadgeageByIdQuery(int Id) : IRequest<BadgeageEntity?>;
