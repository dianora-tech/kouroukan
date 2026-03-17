using MediatR;

namespace Communication.Application.Commands;

/// <summary>
/// Commande de suppression d'une annonce.
/// </summary>
public sealed record DeleteAnnonceCommand(int Id) : IRequest<bool>;
