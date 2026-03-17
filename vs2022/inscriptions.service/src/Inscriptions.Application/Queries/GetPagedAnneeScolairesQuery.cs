using GnDapper.Models;
using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des annees scolaires.
/// </summary>
public sealed record GetPagedAnneeScolairesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy) : IRequest<PagedResult<AnneeScolaire>>;
