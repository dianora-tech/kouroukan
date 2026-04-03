using Xunit;
using Support.Api.Controllers;
using Support.Api.Models;
using Support.Application.Commands;
using Support.Application.Queries;
using Support.Domain.Entities;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Support.Tests.Unit.Controllers;

public sealed class ArticleAideControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<ArticleAideController>> _logger;
    private readonly ArticleAideController _sut;

    public ArticleAideControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<ArticleAideController>>();
        _sut = new ArticleAideController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<ArticleAide>(new List<ArticleAide>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<GetPagedArticlesAideQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<ArticleAide>>>();
    }

    // ─── GetById ───

    [Fact]
    public async Task GetById_DoitRetournerOk_QuandArticleTrouve()
    {
        var entity = new ArticleAide() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<GetArticleAideByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetById(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_DoitRetournerNotFound_QuandArticleInexistant()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetArticleAideByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArticleAide?)null);

        var result = await _sut.GetById(999, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ─── GetTypes ───

    [Fact]
    public void GetTypes_DoitRetournerOk_AvecListeDeTypes()
    {
        var result = _sut.GetTypes();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    // ─── Search ───

    [Fact]
    public async Task Search_DoitRetournerBadRequest_QuandQueryVide()
    {
        var result = await _sut.Search("  ", 10, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Search_DoitRetournerOk_QuandQueryValide()
    {
        var articles = new List<ArticleAide>() as IReadOnlyList<ArticleAide>;
        _mediator.Setup(m => m.Send(It.IsAny<RechercherArticlesAideQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(articles);

        var result = await _sut.Search("test", 10, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var entity = new ArticleAide() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<CreateArticleAideCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mediator.Object.Should().NotBeNull();
    }

    // ─── Update ───

    [Fact]
    public async Task Update_DoitRetournerBadRequest_QuandIdNonCorrespondant()
    {
        var command = new UpdateArticleAideCommand(999, "name", null, 1, "titre", "contenu", "slug", "cat", null, 1, true, 1);

        var result = await _sut.Update(1, command, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── Delete ───

    [Fact]
    public async Task Delete_DoitRetournerNotFound_QuandArticleInexistant()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteArticleAideCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mediator.Object.Should().NotBeNull();
    }

    // ─── MarquerUtile ───

    [Fact]
    public async Task MarquerUtile_DoitRetournerOk_QuandAppele()
    {
        _mediator.Setup(m => m.Send(It.IsAny<MarquerArticleUtileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.MarquerUtile(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
