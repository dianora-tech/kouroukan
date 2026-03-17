using GnDapper.Models;
using MediatR;
using AppelEntity = Presences.Domain.Entities.Appel;

namespace Presences.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des appels.
/// </summary>
public sealed record GetPagedAppelsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<AppelEntity>>;
