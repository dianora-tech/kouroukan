using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Commands;

public sealed record CreateFactureCommand(
    int TypeId,
    int EleveId,
    int? ParentId,
    int AnneeScolaireId,
    decimal MontantTotal,
    decimal MontantPaye,
    DateTime DateEmission,
    DateTime DateEcheance,
    string StatutFacture,
    string NumeroFacture,
    int UserId) : IRequest<Facture>;
