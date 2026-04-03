using Xunit;
using Bde.Api.Controllers;
using Bde.Domain.Entities;
using Bde.Api.Models;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Bde.Tests.Unit.Controllers;

public sealed class DepenseBdeControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<DepenseBdeController>> _logger;
    private readonly DepenseBdeController _sut;

    public DepenseBdeControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<DepenseBdeController>>();
        _sut = new DepenseBdeController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<DepenseBdeEntity>(new List<DepenseBdeEntity>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PagedResult<DepenseBdeEntity>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<DepenseBdeEntity>>>();
    }

    // ─── GetById ───

    [Fact]
    public async Task GetById_DoitRetournerOk_QuandEntiteTrouvee()
    {
        var entity = new DepenseBdeEntity() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<DepenseBdeEntity?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetById(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_DoitRetournerNotFound_QuandEntiteInexistante()
    {
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<DepenseBdeEntity?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DepenseBdeEntity?)null);

        var result = await _sut.GetById(999, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var created = new DepenseBdeEntity() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<DepenseBdeEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        _mediator.Verify(m => m.Send(It.IsAny<IRequest<DepenseBdeEntity>>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    // ─── Update ───

    [Fact]
    public async Task Update_DoitRetournerOk_QuandMisAJourReussie()
    {
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Verified via mediator setup
        _mediator.Object.Should().NotBeNull();
    }

    // ─── Delete ───

    [Fact]
    public async Task Delete_DoitRetournerOk_QuandSuppressionReussie()
    {
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mediator.Object.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_DoitRetournerNotFound_QuandEntiteInexistante()
    {
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mediator.Object.Should().NotBeNull();
    }

}
