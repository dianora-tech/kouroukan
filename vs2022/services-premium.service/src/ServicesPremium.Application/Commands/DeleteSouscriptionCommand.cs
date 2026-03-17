using MediatR;

namespace ServicesPremium.Application.Commands;

/// <summary>
/// Commande de suppression d'une souscription.
/// </summary>
public sealed record DeleteSouscriptionCommand(int Id) : IRequest<bool>;
