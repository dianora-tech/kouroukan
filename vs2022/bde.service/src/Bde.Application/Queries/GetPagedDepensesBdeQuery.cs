using GnDapper.Models;
using MediatR;
using DepenseBdeEntity = Bde.Domain.Entities.DepenseBde;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des depenses BDE.
/// </summary>
public sealed record GetPagedDepensesBdeQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<DepenseBdeEntity>>;
