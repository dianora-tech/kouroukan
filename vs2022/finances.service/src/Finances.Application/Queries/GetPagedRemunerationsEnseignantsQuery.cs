using Finances.Domain.Entities;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Queries;

public sealed record GetPagedRemunerationsEnseignantsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy) : IRequest<PagedResult<RemunerationEnseignant>>;
