using GnDapper.Models;
using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Queries;

/// <summary>
/// Requete de recuperation paginee des messages.
/// </summary>
public sealed record GetPagedMessagesQuery(
    int Page,
    int PageSize,
    string? Search,
    string? OrderBy,
    int? TypeId) : IRequest<PagedResult<Message>>;
