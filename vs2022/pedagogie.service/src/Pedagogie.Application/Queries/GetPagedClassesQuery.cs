using GnDapper.Models;
using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des classes.
/// </summary>
public sealed record GetPagedClassesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy) : IRequest<PagedResult<Classe>>;
