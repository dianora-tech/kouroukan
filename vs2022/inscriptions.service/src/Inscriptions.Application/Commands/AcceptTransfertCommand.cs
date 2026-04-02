using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande d'acceptation d'un transfert.
/// </summary>
public sealed record AcceptTransfertCommand(int Id) : IRequest<bool>;
