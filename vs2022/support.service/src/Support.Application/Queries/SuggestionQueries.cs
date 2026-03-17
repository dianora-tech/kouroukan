using GnDapper.Models;
using MediatR;
using Support.Domain.Entities;

namespace Support.Application.Queries;

public sealed record GetSuggestionByIdQuery(int Id) : IRequest<Suggestion?>;

public sealed record GetAllSuggestionsQuery() : IRequest<IReadOnlyList<Suggestion>>;

public sealed record GetPagedSuggestionsQuery(
    int Page,
    int PageSize,
    string? Search,
    int? TypeId,
    string? OrderBy) : IRequest<PagedResult<Suggestion>>;

public sealed record GetTopSuggestionsQuery(int Limit = 10) : IRequest<IReadOnlyList<Suggestion>>;
