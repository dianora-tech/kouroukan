using GnDapper.Models;
using MediatR;
using AssociationEntity = Bde.Domain.Entities.Association;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des associations.
/// </summary>
public sealed record GetPagedAssociationsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<AssociationEntity>>;
