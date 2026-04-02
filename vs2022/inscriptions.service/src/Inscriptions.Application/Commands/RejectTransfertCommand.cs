using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de refus d'un transfert.
/// </summary>
public sealed record RejectTransfertCommand(int Id) : IRequest<bool>;
