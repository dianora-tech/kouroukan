using GnDapper.Entities;
using GnDapper.Models;
using MediatR;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Application.Queries;

/// <summary>
/// Requete paginee pour les services parents.
/// </summary>
public sealed record GetPagedServiceParentsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<ServiceParent>>;
