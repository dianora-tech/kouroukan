using GnDapper.Models;
using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des niveaux de classes.
/// </summary>
public sealed record GetPagedNiveauClassesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<NiveauClasse>>;
