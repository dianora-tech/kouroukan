using GnDapper.Models;
using MediatR;
using BadgeageEntity = Presences.Domain.Entities.Badgeage;

namespace Presences.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des badgeages.
/// </summary>
public sealed record GetPagedBadgeagesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<BadgeageEntity>>;
