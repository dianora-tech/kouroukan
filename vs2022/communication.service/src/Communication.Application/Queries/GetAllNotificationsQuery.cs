using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les notifications.
/// </summary>
public sealed record GetAllNotificationsQuery() : IRequest<IReadOnlyList<Notification>>;
