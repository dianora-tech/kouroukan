using FluentAssertions;
using Bde.Application.Commands;
using Bde.Application.Handlers;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Bde.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour MembreBdeCommandHandler.
/// </summary>
public sealed class MembreBdeCommandHandlerTests
{
    private readonly Mock<IMembreBdeService> _serviceMock;
    private readonly MembreBdeCommandHandler _sut;

    public MembreBdeCommandHandlerTests()
    {
        _serviceMock = new Mock<IMembreBdeService>();
        _sut = new MembreBdeCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneMembre()
    {
        var command = new CreateMembreBdeCommand(
            Name: "Jean Dupont",
            Description: "Membre actif",
            AssociationId: 5,
            EleveId: 10,
            RoleBde: "Membre",
            DateAdhesion: new DateTime(2025, 9, 1),
            MontantCotisation: 25000m,
            EstActif: true,
            UserId: 1);

        var expected = new MembreBde
        {
            Id = 42,
            Name = "Jean Dupont",
            RoleBde = "Membre"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<MembreBde>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<MembreBde>(m =>
                m.Name == "Jean Dupont" &&
                m.AssociationId == 5 &&
                m.EleveId == 10 &&
                m.RoleBde == "Membre"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateMembreBdeCommand(
            Id: 1, Name: "Jean Dupont Maj", Description: null,
            AssociationId: 5, EleveId: 10, RoleBde: "Tresorier",
            DateAdhesion: new DateTime(2025, 9, 1),
            MontantCotisation: 50000m, EstActif: true, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<MembreBde>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<MembreBde>(m => m.Id == 1 && m.RoleBde == "Tresorier"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateMembreBdeCommand(
            Id: 999, Name: "Test", Description: null,
            AssociationId: 5, EleveId: 10, RoleBde: "Membre",
            DateAdhesion: DateTime.Today, MontantCotisation: null,
            EstActif: true, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<MembreBde>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteMembreBdeCommand(1);

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
        var command = new DeleteMembreBdeCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
