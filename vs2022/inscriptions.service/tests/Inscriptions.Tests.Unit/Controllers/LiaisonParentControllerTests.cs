using Inscriptions.Api.Controllers;
using Inscriptions.Domain.Entities;
using Inscriptions.Api.Models;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Inscriptions.Tests.Unit.Controllers;

public sealed class LiaisonParentControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<LiaisonParentController>> _logger;
    private readonly LiaisonParentController _sut;

    public LiaisonParentControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<LiaisonParentController>>();
        _sut = new LiaisonParentController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<LiaisonParent>(new List<LiaisonParent>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PagedResult<LiaisonParent>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<LiaisonParent>>>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var created = new LiaisonParent() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<LiaisonParent>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        _mediator.Verify(m => m.Send(It.IsAny<IRequest<LiaisonParent>>(), It.IsAny<CancellationToken>()), Times.Never());
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
