using GnDapper.Models;
using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des signatures.
/// </summary>
public sealed record GetPagedSignaturesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<Signature>>;
