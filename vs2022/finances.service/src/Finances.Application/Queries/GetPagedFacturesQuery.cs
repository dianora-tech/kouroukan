using Finances.Domain.Entities;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Queries;

public sealed record GetPagedFacturesQuery(
    int Page,
    int PageSize,
    string? Search,
    int? TypeId,
    string? OrderBy) : IRequest<PagedResult<Facture>>;
