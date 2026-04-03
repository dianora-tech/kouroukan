using FluentAssertions;
using Presences.Application.Commands;
using Presences.Application.Handlers;
using Presences.Domain.Entities;
using Presences.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Presences.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AppelCommandHandler.
/// </summary>
public sealed class AppelCommandHandlerTests
{
    private readonly Mock<IAppelService> _serviceMock;
    private readonly AppelCommandHandler _sut;

    public AppelCommandHandlerTests()
    {
        _serviceMock = new Mock<IAppelService>();
        _sut = new AppelCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneAppel()
    {
        var command = new CreateAppelCommand(
            ClasseId: 5,
            EnseignantId: 3,
            SeanceId: null,
            DateAppel: new DateTime(2025, 9, 15),
            HeureAppel: new TimeSpan(8, 0, 0),
            EstCloture: false,
            UserId: 1);

        var expected = new Appel
        {
            Id = 42,
            ClasseId = 5,
            EnseignantId = 3,
            EstCloture = false
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Appel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Appel>(a =>
                a.ClasseId == 5 &&
                a.EnseignantId == 3 &&
                !a.EstCloture),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateAppelCommand(
            Id: 1,
            ClasseId: 5,
            EnseignantId: 3,
            SeanceId: 10,
            DateAppel: new DateTime(2025, 9, 15),
            HeureAppel: new TimeSpan(8, 0, 0),
            EstCloture: true,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Appel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Appel>(a => a.Id == 1 && a.EstCloture),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateAppelCommand(
            Id: 999, ClasseId: 5, EnseignantId: 3, SeanceId: null,
            DateAppel: DateTime.Today, HeureAppel: new TimeSpan(8, 0, 0),
            EstCloture: false, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Appel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteAppelCommand(1);

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
        var command = new DeleteAppelCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
