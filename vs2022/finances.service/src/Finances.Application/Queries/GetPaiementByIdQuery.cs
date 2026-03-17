using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Queries;

public sealed record GetPaiementByIdQuery(int Id) : IRequest<Paiement?>;
