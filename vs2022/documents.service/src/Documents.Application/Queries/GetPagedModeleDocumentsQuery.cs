using GnDapper.Models;
using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des modeles de documents.
/// </summary>
public sealed record GetPagedModeleDocumentsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<ModeleDocument>>;
