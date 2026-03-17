using MediatR;

namespace Evaluations.Application.Commands;

/// <summary>
/// Commande de suppression d'un bulletin.
/// </summary>
public sealed record DeleteBulletinCommand(int Id) : IRequest<bool>;
