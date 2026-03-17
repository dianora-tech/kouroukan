using GnDapper.Models;
using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des documents generes.
/// </summary>
public sealed record GetPagedDocumentGeneresQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<DocumentGenere>>;
