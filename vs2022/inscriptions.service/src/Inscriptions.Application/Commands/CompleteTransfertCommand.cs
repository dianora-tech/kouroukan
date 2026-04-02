using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de completion d'un transfert.
/// </summary>
public sealed record CompleteTransfertCommand(int Id) : IRequest<bool>;
