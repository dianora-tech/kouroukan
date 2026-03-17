using MediatR;

namespace Communication.Application.Commands;

/// <summary>
/// Commande de suppression d'un message.
/// </summary>
public sealed record DeleteMessageCommand(int Id) : IRequest<bool>;
