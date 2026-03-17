using MediatR;

namespace Presences.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un badgeage.
/// </summary>
public sealed record UpdateBadgeageCommand(
    int Id,
    int TypeId,
    int EleveId,
    DateTime DateBadgeage,
    TimeSpan HeureBadgeage,
    string PointAcces,
    string MethodeBadgeage,
    int UserId) : IRequest<bool>;
