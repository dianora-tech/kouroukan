using GnDapper.Models;
using Personnel.Domain.Entities;
using MediatR;

namespace Personnel.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des enseignants.
/// </summary>
public sealed record GetPagedEnseignantsQuery(
    int Page,
    int PageSize,
    string? Search,
    int? TypeId,
    string? OrderBy) : IRequest<PagedResult<Enseignant>>;
