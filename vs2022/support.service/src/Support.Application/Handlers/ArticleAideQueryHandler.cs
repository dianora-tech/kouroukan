using GnDapper.Models;
using MediatR;
using Support.Application.Queries;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;

namespace Support.Application.Handlers;

/// <summary>
/// Handler pour les requetes d'articles d'aide.
/// </summary>
public sealed class ArticleAideQueryHandler :
    IRequestHandler<GetArticleAideByIdQuery, ArticleAide?>,
    IRequestHandler<GetAllArticlesAideQuery, IReadOnlyList<ArticleAide>>,
    IRequestHandler<GetPagedArticlesAideQuery, PagedResult<ArticleAide>>,
    IRequestHandler<RechercherArticlesAideQuery, IReadOnlyList<ArticleAide>>
{
    private readonly IArticleAideService _service;

    public ArticleAideQueryHandler(IArticleAideService service) => _service = service;

    public async Task<ArticleAide?> Handle(GetArticleAideByIdQuery request, CancellationToken ct) =>
        await _service.GetByIdAsync(request.Id, ct);

    public async Task<IReadOnlyList<ArticleAide>> Handle(GetAllArticlesAideQuery request, CancellationToken ct) =>
        await _service.GetAllAsync(ct);

    public async Task<PagedResult<ArticleAide>> Handle(GetPagedArticlesAideQuery request, CancellationToken ct) =>
        await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct);

    public async Task<IReadOnlyList<ArticleAide>> Handle(RechercherArticlesAideQuery request, CancellationToken ct) =>
        await _service.RechercherFullTextAsync(request.Query, request.Limit, ct);
}
