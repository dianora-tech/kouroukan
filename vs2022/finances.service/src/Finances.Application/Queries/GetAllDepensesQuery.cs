using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Queries;

public sealed record GetAllDepensesQuery : IRequest<IReadOnlyList<Depense>>;
