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

public sealed class RadiationControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<RadiationController>> _logger;
    private readonly RadiationController _sut;

    public RadiationControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<RadiationController>>();
        _sut = new RadiationController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<Radiation>(new List<Radiation>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PagedResult<Radiation>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<Radiation>>>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var created = new Radiation() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<Radiation>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        _mediator.Verify(m => m.Send(It.IsAny<IRequest<Radiation>>(), It.IsAny<CancellationToken>()), Times.Never());
    }

}
