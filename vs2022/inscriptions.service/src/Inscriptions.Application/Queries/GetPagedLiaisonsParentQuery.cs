using GnDapper.Models;
using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des liaisons parent.
/// </summary>
public sealed record GetPagedLiaisonsParentQuery(
    int Page,
    int PageSize,
    int? ParentUserId,
    int? CompanyId,
    string? OrderBy) : IRequest<PagedResult<LiaisonParent>>;
