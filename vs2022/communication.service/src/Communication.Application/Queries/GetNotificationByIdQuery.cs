using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Queries;

/// <summary>
/// Requete de recuperation d'une notification par son identifiant.
/// </summary>
public sealed record GetNotificationByIdQuery(int Id) : IRequest<Notification?>;
