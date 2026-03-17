using GnDapper.Models;
using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des eleves.
/// </summary>
public sealed record GetPagedElevesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy) : IRequest<PagedResult<Eleve>>;
