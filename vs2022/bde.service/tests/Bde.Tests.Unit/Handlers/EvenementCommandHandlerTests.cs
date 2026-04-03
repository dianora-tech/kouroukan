using FluentAssertions;
using Bde.Application.Commands;
using Bde.Application.Handlers;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Bde.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour EvenementCommandHandler.
/// </summary>
public sealed class EvenementCommandHandlerTests
{
    private readonly Mock<IEvenementService> _serviceMock;
    private readonly EvenementCommandHandler _sut;

    public EvenementCommandHandlerTests()
    {
        _serviceMock = new Mock<IEvenementService>();
        _sut = new EvenementCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneEvenement()
    {
        var command = new CreateEvenementCommand(
            TypeId: 1,
            Name: "Journee integration",
            Description: "Bienvenue",
            AssociationId: 5,
            DateEvenement: new DateTime(2025, 10, 15),
            Lieu: "Campus principal",
            Capacite: 200,
            TarifEntree: 5000m,
            NombreInscrits: 0,
            StatutEvenement: "Planifie",
            UserId: 1);

        var expected = new Evenement
        {
            Id = 42,
            TypeId = 1,
            Name = "Journee integration",
            StatutEvenement = "Planifie"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Evenement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Evenement>(e =>
                e.TypeId == 1 &&
                e.AssociationId == 5 &&
                e.Lieu == "Campus principal" &&
                e.StatutEvenement == "Planifie"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateEvenementCommand(
            Id: 1, TypeId: 1, Name: "Evenement maj", Description: null,
            AssociationId: 5, DateEvenement: DateTime.Today,
            Lieu: "Salle B", Capacite: 100, TarifEntree: null,
            NombreInscrits: 50, StatutEvenement: "Valide", UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Evenement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Evenement>(e => e.Id == 1 && e.StatutEvenement == "Valide"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateEvenementCommand(
            Id: 999, TypeId: 1, Name: "Test", Description: null,
            AssociationId: 5, DateEvenement: DateTime.Today,
            Lieu: "Lieu", Capacite: null, TarifEntree: null,
            NombreInscrits: 0, StatutEvenement: "Planifie", UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Evenement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteEvenementCommand(1);

        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistant()
    {
        var command = new DeleteEvenementCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
