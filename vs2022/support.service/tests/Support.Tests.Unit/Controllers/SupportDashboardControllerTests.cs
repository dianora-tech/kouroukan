using Support.Api.Controllers;
using Support.Api.Models;
using Support.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Support.Tests.Unit.Controllers;

public sealed class SupportDashboardControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<SupportDashboardController>> _logger;
    private readonly SupportDashboardController _sut;

    public SupportDashboardControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<SupportDashboardController>>();
        _sut = new SupportDashboardController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetDashboard ───

    [Fact]
    public async Task GetDashboard_DoitRetournerOk_QuandAppele()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetSupportDashboardQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SupportDashboardResult());

        var result = await _sut.GetDashboard(CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
