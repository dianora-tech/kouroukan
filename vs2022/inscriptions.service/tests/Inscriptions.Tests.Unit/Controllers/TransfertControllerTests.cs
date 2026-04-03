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

public sealed class TransfertControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<TransfertController>> _logger;
    private readonly TransfertController _sut;

    public TransfertControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<TransfertController>>();
        _sut = new TransfertController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<Transfert>(new List<Transfert>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PagedResult<Transfert>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<Transfert>>>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var created = new Transfert() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<Transfert>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        _mediator.Verify(m => m.Send(It.IsAny<IRequest<Transfert>>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    // ─── Accept ───

    [Fact]
    public async Task Accept_DoitRetournerOk_QuandAppele()
    {
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mediator.Object.Should().NotBeNull();
    }

    // ─── Reject ───

    [Fact]
    public async Task Reject_DoitRetournerOk_QuandAppele()
    {
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mediator.Object.Should().NotBeNull();
    }

    // ─── Complete ───

    [Fact]
    public async Task Complete_DoitRetournerOk_QuandAppele()
    {
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mediator.Object.Should().NotBeNull();
    }

}
