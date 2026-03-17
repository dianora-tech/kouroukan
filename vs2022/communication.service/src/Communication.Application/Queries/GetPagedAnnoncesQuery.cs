using GnDapper.Models;
using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des annonces.
/// </summary>
public sealed record GetPagedAnnoncesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<Annonce>>;
