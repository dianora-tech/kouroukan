using GnDapper.Models;
using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des radiations.
/// </summary>
public sealed record GetPagedRadiationsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy) : IRequest<PagedResult<Radiation>>;
