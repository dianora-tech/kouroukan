using MediatR;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de suppression d'un evenement.
/// </summary>
public sealed record DeleteEvenementCommand(int Id) : IRequest<bool>;
