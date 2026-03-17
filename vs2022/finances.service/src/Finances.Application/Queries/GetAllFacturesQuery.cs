using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Queries;

public sealed record GetAllFacturesQuery : IRequest<IReadOnlyList<Facture>>;
