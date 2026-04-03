using FluentAssertions;
using Presences.Application.Commands;
using Presences.Application.Handlers;
using Presences.Domain.Entities;
using Presences.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Presences.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AbsenceCommandHandler.
/// </summary>
public sealed class AbsenceCommandHandlerTests
{
    private readonly Mock<IAbsenceService> _serviceMock;
    private readonly AbsenceCommandHandler _sut;

    public AbsenceCommandHandlerTests()
    {
        _serviceMock = new Mock<IAbsenceService>();
        _sut = new AbsenceCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneAbsence()
    {
        var command = new CreateAbsenceCommand(
            TypeId: 1,
            EleveId: 10,
            AppelId: 5,
            DateAbsence: new DateTime(2025, 9, 15),
            HeureDebut: new TimeSpan(8, 0, 0),
            HeureFin: new TimeSpan(10, 0, 0),
            EstJustifiee: false,
            MotifJustification: null,
            PieceJointeUrl: null,
            UserId: 1);

        var expected = new Absence
        {
            Id = 42,
            TypeId = 1,
            EleveId = 10,
            AppelId = 5,
            EstJustifiee = false
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Absence>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Absence>(a =>
                a.TypeId == 1 &&
                a.EleveId == 10 &&
                a.AppelId == 5 &&
                !a.EstJustifiee),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateAbsenceCommand(
            Id: 1,
            TypeId: 1,
            EleveId: 10,
            AppelId: null,
            DateAbsence: new DateTime(2025, 9, 15),
            HeureDebut: new TimeSpan(8, 0, 0),
            HeureFin: new TimeSpan(10, 0, 0),
            EstJustifiee: true,
            MotifJustification: "Certificat medical",
            PieceJointeUrl: null,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Absence>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Absence>(a => a.Id == 1 && a.EstJustifiee),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateAbsenceCommand(
            Id: 999, TypeId: 1, EleveId: 10, AppelId: null,
            DateAbsence: DateTime.Today, HeureDebut: null, HeureFin: null,
            EstJustifiee: false, MotifJustification: null, PieceJointeUrl: null,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Absence>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteAbsenceCommand(1);

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
        var command = new DeleteAbsenceCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
