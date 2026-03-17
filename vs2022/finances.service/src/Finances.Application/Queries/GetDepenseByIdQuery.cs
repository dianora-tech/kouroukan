using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Queries;

public sealed record GetDepenseByIdQuery(int Id) : IRequest<Depense?>;
