using FluentAssertions;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Handlers;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour SeanceCommandHandler.
/// </summary>
public sealed class SeanceCommandHandlerTests
{
    private readonly Mock<ISeanceService> _serviceMock;
    private readonly SeanceCommandHandler _sut;

    public SeanceCommandHandlerTests()
    {
        _serviceMock = new Mock<ISeanceService>();
        _sut = new SeanceCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneSeance()
    {
        var command = new CreateSeanceCommand(
            Name: "Maths 7A Lundi 8h",
            Description: "Cours de maths",
            MatiereId: 1,
            ClasseId: 1,
            EnseignantId: 1,
            SalleId: 1,
            JourSemaine: 1,
            HeureDebut: TimeSpan.FromHours(8),
            HeureFin: TimeSpan.FromHours(10),
            AnneeScolaireId: 1);

        var expected = new Seance
        {
            Id = 42,
            Name = "Maths 7A Lundi 8h",
            MatiereId = 1
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Seance>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Seance>(se =>
                se.Name == "Maths 7A Lundi 8h" &&
                se.MatiereId == 1 &&
                se.JourSemaine == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateSeanceCommand(
            Id: 1, Name: "Maths 7A Lundi 8h", Description: null,
            MatiereId: 1, ClasseId: 1, EnseignantId: 1, SalleId: 1,
            JourSemaine: 1, HeureDebut: TimeSpan.FromHours(8),
            HeureFin: TimeSpan.FromHours(10), AnneeScolaireId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Seance>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Seance>(se => se.Id == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateSeanceCommand(
            Id: 999, Name: "X", Description: null,
            MatiereId: 1, ClasseId: 1, EnseignantId: 1, SalleId: 1,
            JourSemaine: 1, HeureDebut: TimeSpan.FromHours(8),
            HeureFin: TimeSpan.FromHours(10), AnneeScolaireId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Seance>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteSeanceCommand(1);

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
        var command = new DeleteSeanceCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
