using GnDapper.Models;
using MediatR;
using Support.Application.Queries;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Support.Domain.Ports.Output;

namespace Support.Application.Handlers;

/// <summary>
/// Handler pour les requetes de suggestions.
/// </summary>
public sealed class SuggestionQueryHandler :
    IRequestHandler<GetSuggestionByIdQuery, Suggestion?>,
    IRequestHandler<GetAllSuggestionsQuery, IReadOnlyList<Suggestion>>,
    IRequestHandler<GetPagedSuggestionsQuery, PagedResult<Suggestion>>,
    IRequestHandler<GetTopSuggestionsQuery, IReadOnlyList<Suggestion>>
{
    private readonly ISuggestionService _service;
    private readonly ISuggestionRepository _repository;

    public SuggestionQueryHandler(ISuggestionService service, ISuggestionRepository repository)
    {
        _service = service;
        _repository = repository;
    }

    public async Task<Suggestion?> Handle(GetSuggestionByIdQuery request, CancellationToken ct) =>
        await _service.GetByIdAsync(request.Id, ct);

    public async Task<IReadOnlyList<Suggestion>> Handle(GetAllSuggestionsQuery request, CancellationToken ct) =>
        await _service.GetAllAsync(ct);

    public async Task<PagedResult<Suggestion>> Handle(GetPagedSuggestionsQuery request, CancellationToken ct) =>
        await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct);

    public async Task<IReadOnlyList<Suggestion>> Handle(GetTopSuggestionsQuery request, CancellationToken ct) =>
        await _repository.GetTopVoteesAsync(request.Limit, ct);
}
