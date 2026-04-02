using MediatR;

namespace Finances.Application.Commands;

/// <summary>
/// Commande de suppression logique d'un moyen de paiement.
/// </summary>
public sealed record DeleteMoyenPaiementCommand(int Id) : IRequest<bool>;
