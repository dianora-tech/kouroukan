using GnDapper.Models;
using Personnel.Domain.Entities;
using MediatR;

namespace Personnel.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des demandes de conge.
/// </summary>
public sealed record GetPagedDemandesCongesQuery(
    int Page,
    int PageSize,
    string? Search,
    int? TypeId,
    string? OrderBy) : IRequest<PagedResult<DemandeConge>>;
