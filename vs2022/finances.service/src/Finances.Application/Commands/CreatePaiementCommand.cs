using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Commands;

public sealed record CreatePaiementCommand(
    int TypeId,
    int FactureId,
    decimal MontantPaye,
    DateTime DatePaiement,
    string MoyenPaiement,
    string? ReferenceMobileMoney,
    string StatutPaiement,
    int? CaissierId,
    string NumeroRecu,
    int UserId) : IRequest<Paiement>;
