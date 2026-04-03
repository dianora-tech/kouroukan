using FluentAssertions;
using GnDapper.Models;
using Support.Application.Handlers;
using Support.Application.Queries;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Support.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour TicketQueryHandler.
/// </summary>
public sealed class TicketQueryHandlerTests
{
    private readonly Mock<ITicketService> _serviceMock;
    private readonly TicketQueryHandler _sut;

    public TicketQueryHandlerTests()
    {
        _serviceMock = new Mock<ITicketService>();
        _sut = new TicketQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetById_RetourneTicket()
    {
        var ticket = new Ticket { Id = 1, Sujet = "Test" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var result = await _sut.Handle(new GetTicketByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket?)null);

        var result = await _sut.Handle(new GetTicketByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var tickets = new List<Ticket>
        {
            new() { Id = 1, Sujet = "Ticket 1" },
            new() { Id = 2, Sujet = "Ticket 2" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);

        var result = await _sut.Handle(new GetAllTicketsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListeVide()
    {
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Ticket>());

        var result = await _sut.Handle(new GetAllTicketsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Ticket>(
            new List<Ticket> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedTicketsQuery(1, 20, "test", null, "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── GetReponses ───

    [Fact]
    public async Task Handle_GetReponses_RetourneListe()
    {
        var reponses = new List<ReponseTicket>
        {
            new() { Id = 1, Contenu = "Reponse 1" },
            new() { Id = 2, Contenu = "Reponse 2" },
        };
        _serviceMock
            .Setup(s => s.GetReponsesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reponses);

        var result = await _sut.Handle(new GetReponsesTicketQuery(1), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetReponses_RetourneListeVide_SiAucuneReponse()
    {
        _serviceMock
            .Setup(s => s.GetReponsesAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReponseTicket>());

        var result = await _sut.Handle(new GetReponsesTicketQuery(999), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
