using GnDapper.Models;
using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des bulletins.
/// </summary>
public sealed record GetPagedBulletinsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy) : IRequest<PagedResult<Bulletin>>;
