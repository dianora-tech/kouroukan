using GnDapper.Models;
using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des notes.
/// </summary>
public sealed record GetPagedNotesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy) : IRequest<PagedResult<Note>>;
