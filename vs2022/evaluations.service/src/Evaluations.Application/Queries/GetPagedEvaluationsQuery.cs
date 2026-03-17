using GnDapper.Models;
using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des evaluations.
/// </summary>
public sealed record GetPagedEvaluationsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<Evaluation>>;
