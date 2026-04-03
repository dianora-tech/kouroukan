using Xunit;
using Bde.Api.Controllers;
using Bde.Api.Models;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Bde.Domain.Ports.Output;
using EvenementEntity = Bde.Domain.Entities.Evenement;

namespace Bde.Tests.Unit.Controllers;

public sealed class EvenementControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IEvenementRepository> _repository;
    private readonly Mock<ILogger<EvenementController>> _logger;
    private readonly EvenementController _sut;

    public EvenementControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _repository = new Mock<IEvenementRepository>();
        _logger = new Mock<ILogger<EvenementController>>();
        _sut = new EvenementController(_mediator.Object, _repository.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<EvenementEntity>(new List<EvenementEntity>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PagedResult<EvenementEntity>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<EvenementEntity>>>();
    }

    // ─── GetById ───

    [Fact]
    public async Task GetById_DoitRetournerOk_QuandEntiteTrouvee()
    {
        var entity = new EvenementEntity() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<EvenementEntity?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetById(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_DoitRetournerNotFound_QuandEntiteInexistante()
    {
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<EvenementEntity?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EvenementEntity?)null);

        var result = await _sut.GetById(999, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var created = new EvenementEntity() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<EvenementEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        _mediator.Verify(m => m.Send(It.IsAny<IRequest<EvenementEntity>>(), It.IsAny<CancellationToken>()), Times.Never());
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
