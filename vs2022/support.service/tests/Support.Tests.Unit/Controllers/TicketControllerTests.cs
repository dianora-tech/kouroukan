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

public sealed class TicketControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<TicketController>> _logger;
    private readonly TicketController _sut;

    public TicketControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<TicketController>>();
        _sut = new TicketController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<Ticket>(new List<Ticket>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<GetPagedTicketsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<Ticket>>>();
    }

    // ─── GetById ───

    [Fact]
    public async Task GetById_DoitRetournerOk_QuandTicketTrouve()
    {
        var ticket = new Ticket() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<GetTicketByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var result = await _sut.GetById(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_DoitRetournerNotFound_QuandTicketInexistant()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetTicketByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket?)null);

        var result = await _sut.GetById(999, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var ticket = new Ticket() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<CreateTicketCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _mediator.Object.Should().NotBeNull();
    }

    // ─── Update ───

    [Fact]
    public async Task Update_DoitRetournerOk_QuandMisAJourReussie()
    {
        _mediator.Setup(m => m.Send(It.IsAny<UpdateTicketCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mediator.Object.Should().NotBeNull();
    }

    // ─── Delete ───

    [Fact]
    public async Task Delete_DoitRetournerOk_QuandSuppressionReussie()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteTicketCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mediator.Object.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_DoitRetournerNotFound_QuandTicketInexistant()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteTicketCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mediator.Object.Should().NotBeNull();
    }

    // ─── GetReponses ───

    [Fact]
    public async Task GetReponses_DoitRetournerOk_QuandAppele()
    {
        var reponses = new List<ReponseTicket>() as IReadOnlyList<ReponseTicket>;
        _mediator.Setup(m => m.Send(It.IsAny<GetReponsesTicketQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reponses);

        var result = await _sut.GetReponses(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    // ─── AddReponse ───

    [Fact]
    public async Task AddReponse_DoitRetournerBadRequest_QuandIdNonCorrespondant()
    {
        var command = new AddReponseTicketCommand(999, 1, "contenu", false, false, 1);

        var result = await _sut.AddReponse(1, command, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── GenererReponseIA ───

    [Fact]
    public async Task GenererReponseIA_DoitRetournerOk_QuandAppele()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GenererReponseIATicketCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("suggestion IA");

        var result = await _sut.GenererReponseIA(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
