using MediatR;
using BadgeageEntity = Presences.Domain.Entities.Badgeage;

namespace Presences.Application.Commands;

/// <summary>
/// Commande de creation d'un badgeage.
/// </summary>
public sealed record CreateBadgeageCommand(
    int TypeId,
    int EleveId,
    DateTime DateBadgeage,
    TimeSpan HeureBadgeage,
    string PointAcces,
    string MethodeBadgeage,
    int UserId) : IRequest<BadgeageEntity>;
