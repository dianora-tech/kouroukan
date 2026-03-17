using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Support.Domain.Ports.Output;
using Support.Domain.Services;
using Xunit;

namespace Support.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour TicketService.
/// </summary>
public sealed class TicketServiceTests
{
    private readonly Mock<ITicketRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<TicketService>> _loggerMock;
    private readonly TicketService _sut;

    public TicketServiceTests()
    {
        _repoMock = new Mock<ITicketRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<TicketService>>();

        _sut = new TicketService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_StatutInitial_EstOuvert()
    {
        var ticket = new Ticket
        {
            Sujet = "Probleme de connexion",
            Contenu = "Je ne peux pas me connecter",
            Priorite = "Moyenne",
            StatutTicket = "Ouvert",
            CategorieTicket = "Bug",
            AuteurId = 1,
            UserId = 1
        };

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var result = await _sut.CreateAsync(ticket);

        result.Should().NotBeNull();
        result.StatutTicket.Should().Be("Ouvert");
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement()
    {
        var ticket = CreateTicket();

        _repoMock.Setup(r => r.AddAsync(ticket, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        await _sut.CreateAsync(ticket);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Ticket>>(),
            "kouroukan.events",
            "entity.created.ticket",
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneTicket_QuandExiste()
    {
        var ticket = CreateTicket();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_RetourneTrue_EtPublieEvenement()
    {
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.DeleteAsync(1);

        result.Should().BeTrue();
        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityDeletedEvent<Ticket>>(),
            "kouroukan.events",
            "entity.deleted.ticket",
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Ticket CreateTicket()
    {
        return new Ticket
        {
            Id = 1,
            Sujet = "Test ticket",
            Contenu = "Contenu du ticket",
            Priorite = "Moyenne",
            StatutTicket = "Ouvert",
            CategorieTicket = "Bug",
            AuteurId = 1,
            UserId = 1
        };
    }
}
