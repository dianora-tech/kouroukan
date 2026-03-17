using MediatR;

namespace Communication.Application.Commands;

/// <summary>
/// Commande de suppression d'une notification.
/// </summary>
public sealed record DeleteNotificationCommand(int Id) : IRequest<bool>;
