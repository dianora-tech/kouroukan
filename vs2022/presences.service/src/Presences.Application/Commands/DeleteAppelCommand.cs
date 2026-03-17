using MediatR;

namespace Presences.Application.Commands;

/// <summary>
/// Commande de suppression d'un appel.
/// </summary>
public sealed record DeleteAppelCommand(int Id) : IRequest<bool>;
