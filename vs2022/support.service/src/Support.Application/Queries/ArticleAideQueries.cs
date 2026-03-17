using GnDapper.Models;
using MediatR;
using Support.Domain.Entities;

namespace Support.Application.Queries;

public sealed record GetArticleAideByIdQuery(int Id) : IRequest<ArticleAide?>;

public sealed record GetAllArticlesAideQuery() : IRequest<IReadOnlyList<ArticleAide>>;

public sealed record GetPagedArticlesAideQuery(
    int Page,
    int PageSize,
    string? Search,
    int? TypeId,
    string? OrderBy) : IRequest<PagedResult<ArticleAide>>;

public sealed record RechercherArticlesAideQuery(
    string Query,
    int Limit = 10) : IRequest<IReadOnlyList<ArticleAide>>;
