using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de suppression d'une liaison parent.
/// </summary>
public sealed record DeleteLiaisonParentCommand(int Id) : IRequest<bool>;
