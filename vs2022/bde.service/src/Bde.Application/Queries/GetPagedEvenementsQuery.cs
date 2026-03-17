using GnDapper.Models;
using MediatR;
using EvenementEntity = Bde.Domain.Entities.Evenement;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des evenements.
/// </summary>
public sealed record GetPagedEvenementsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<EvenementEntity>>;
