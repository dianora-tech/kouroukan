using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de suppression d'une inscription.
/// </summary>
public sealed record DeleteInscriptionCommand(int Id) : IRequest<bool>;
