using FluentAssertions;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Handlers;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour SalleCommandHandler.
/// </summary>
public sealed class SalleCommandHandlerTests
{
    private readonly Mock<ISalleService> _serviceMock;
    private readonly SalleCommandHandler _sut;

    public SalleCommandHandlerTests()
    {
        _serviceMock = new Mock<ISalleService>();
        _sut = new SalleCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneSalle()
    {
        var command = new CreateSalleCommand(
            Name: "Salle 101",
            Description: "Salle principale",
            TypeId: 1,
            Capacite: 40,
            Batiment: "Batiment A",
            Equipements: "Tableau",
            EstDisponible: true);

        var expected = new Salle
        {
            Id = 42,
            Name = "Salle 101",
            Capacite = 40
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Salle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Salle>(sl =>
                sl.Name == "Salle 101" &&
                sl.Capacite == 40 &&
                sl.EstDisponible),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateSalleCommand(
            Id: 1, Name: "Salle 101", Description: null,
            TypeId: 1, Capacite: 40, Batiment: null,
            Equipements: null, EstDisponible: true);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Salle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Salle>(sl => sl.Id == 1 && sl.Name == "Salle 101"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateSalleCommand(
            Id: 999, Name: "X", Description: null,
            TypeId: 1, Capacite: 10, Batiment: null,
            Equipements: null, EstDisponible: false);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Salle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteSalleCommand(1);

        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistante()
    {
        var command = new DeleteSalleCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
