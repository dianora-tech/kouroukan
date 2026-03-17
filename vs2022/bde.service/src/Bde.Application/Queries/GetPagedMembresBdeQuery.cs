using GnDapper.Models;
using MediatR;
using MembreBdeEntity = Bde.Domain.Entities.MembreBde;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des membres BDE.
/// </summary>
public sealed record GetPagedMembresBdeQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy) : IRequest<PagedResult<MembreBdeEntity>>;
