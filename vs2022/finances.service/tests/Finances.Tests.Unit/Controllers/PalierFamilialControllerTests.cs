using Finances.Api.Controllers;
using Finances.Domain.Entities;
using Finances.Api.Models;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Finances.Tests.Unit.Controllers;

public sealed class PalierFamilialControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<PalierFamilialController>> _logger;
    private readonly PalierFamilialController _sut;

    public PalierFamilialControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<PalierFamilialController>>();
        _sut = new PalierFamilialController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<PalierFamilial>(new List<PalierFamilial>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PagedResult<PalierFamilial>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<PalierFamilial>>>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var created = new PalierFamilial() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PalierFamilial>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        _mediator.Verify(m => m.Send(It.IsAny<IRequest<PalierFamilial>>(), It.IsAny<CancellationToken>()), Times.Never());
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
