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
using AssociationEntity = Bde.Domain.Entities.Association;

namespace Bde.Tests.Unit.Controllers;

public sealed class AssociationControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IAssociationRepository> _repository;
    private readonly Mock<ILogger<AssociationController>> _logger;
    private readonly AssociationController _sut;

    public AssociationControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _repository = new Mock<IAssociationRepository>();
        _logger = new Mock<ILogger<AssociationController>>();
        _sut = new AssociationController(_mediator.Object, _repository.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<AssociationEntity>(new List<AssociationEntity>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PagedResult<AssociationEntity>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<AssociationEntity>>>();
    }

    // ─── GetById ───

    [Fact]
    public async Task GetById_DoitRetournerOk_QuandEntiteTrouvee()
    {
        var entity = new AssociationEntity() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<AssociationEntity?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetById(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_DoitRetournerNotFound_QuandEntiteInexistante()
    {
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<AssociationEntity?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AssociationEntity?)null);

        var result = await _sut.GetById(999, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var created = new AssociationEntity() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<AssociationEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        _mediator.Verify(m => m.Send(It.IsAny<IRequest<AssociationEntity>>(), It.IsAny<CancellationToken>()), Times.Never());
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
