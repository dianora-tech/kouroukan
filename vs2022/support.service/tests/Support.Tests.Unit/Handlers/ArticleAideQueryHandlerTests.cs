using FluentAssertions;
using GnDapper.Models;
using Support.Application.Handlers;
using Support.Application.Queries;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Support.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour ArticleAideQueryHandler.
/// </summary>
public sealed class ArticleAideQueryHandlerTests
{
    private readonly Mock<IArticleAideService> _serviceMock;
    private readonly ArticleAideQueryHandler _sut;

    public ArticleAideQueryHandlerTests()
    {
        _serviceMock = new Mock<IArticleAideService>();
        _sut = new ArticleAideQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetById_RetourneArticle()
    {
        var article = new ArticleAide { Id = 1, Titre = "Test" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(article);

        var result = await _sut.Handle(new GetArticleAideByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArticleAide?)null);

        var result = await _sut.Handle(new GetArticleAideByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var articles = new List<ArticleAide>
        {
            new() { Id = 1, Titre = "Article 1" },
            new() { Id = 2, Titre = "Article 2" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(articles);

        var result = await _sut.Handle(new GetAllArticlesAideQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListeVide()
    {
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>());

        var result = await _sut.Handle(new GetAllArticlesAideQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<ArticleAide>(
            new List<ArticleAide> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedArticlesAideQuery(1, 20, "test", null, "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── RechercherFullText ───

    [Fact]
    public async Task Handle_Rechercher_RetourneArticles()
    {
        var articles = new List<ArticleAide>
        {
            new() { Id = 1, Titre = "Connexion" },
            new() { Id = 2, Titre = "Mot de passe" },
        };
        _serviceMock
            .Setup(s => s.RechercherFullTextAsync("connexion", 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(articles);

        var result = await _sut.Handle(
            new RechercherArticlesAideQuery("connexion", 5),
            CancellationToken.None);

        result.Should().HaveCount(2);
        _serviceMock.Verify(s => s.RechercherFullTextAsync("connexion", 5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Rechercher_RetourneListeVide_SiAucunResultat()
    {
        _serviceMock
            .Setup(s => s.RechercherFullTextAsync("xyz", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>());

        var result = await _sut.Handle(
            new RechercherArticlesAideQuery("xyz", 10),
            CancellationToken.None);

        result.Should().BeEmpty();
    }
}
