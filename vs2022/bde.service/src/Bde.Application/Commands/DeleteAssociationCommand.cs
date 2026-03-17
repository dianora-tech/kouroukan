using MediatR;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de suppression d'une association.
/// </summary>
public sealed record DeleteAssociationCommand(int Id) : IRequest<bool>;
