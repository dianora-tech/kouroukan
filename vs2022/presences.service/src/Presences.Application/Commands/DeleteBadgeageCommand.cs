using MediatR;

namespace Presences.Application.Commands;

/// <summary>
/// Commande de suppression d'un badgeage.
/// </summary>
public sealed record DeleteBadgeageCommand(int Id) : IRequest<bool>;
