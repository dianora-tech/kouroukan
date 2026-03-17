using MediatR;

namespace ServicesPremium.Application.Commands;

/// <summary>
/// Commande de suppression d'un service parent.
/// </summary>
public sealed record DeleteServiceParentCommand(int Id) : IRequest<bool>;
