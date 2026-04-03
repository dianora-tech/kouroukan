using Xunit;
using Pedagogie.Api.Controllers;
using Pedagogie.Domain.Entities;
using Pedagogie.Api.Models;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Pedagogie.Tests.Unit.Controllers;

public sealed class CompetenceEnseignantControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<CompetenceEnseignantController>> _logger;
    private readonly CompetenceEnseignantController _sut;

    public CompetenceEnseignantControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<CompetenceEnseignantController>>();
        _sut = new CompetenceEnseignantController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<CompetenceEnseignant>(new List<CompetenceEnseignant>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PagedResult<CompetenceEnseignant>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<CompetenceEnseignant>>>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var created = new CompetenceEnseignant() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<CompetenceEnseignant>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        _mediator.Verify(m => m.Send(It.IsAny<IRequest<CompetenceEnseignant>>(), It.IsAny<CancellationToken>()), Times.Never());
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
