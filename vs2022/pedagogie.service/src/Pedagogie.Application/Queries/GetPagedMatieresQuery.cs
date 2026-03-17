using GnDapper.Models;
using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des matieres.
/// </summary>
public sealed record GetPagedMatieresQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<Matiere>>;
