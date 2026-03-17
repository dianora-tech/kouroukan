using GnDapper.Models;
using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des salles.
/// </summary>
public sealed record GetPagedSallesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<Salle>>;
