using MediatR;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de suppression d'un membre BDE.
/// </summary>
public sealed record DeleteMembreBdeCommand(int Id) : IRequest<bool>;
