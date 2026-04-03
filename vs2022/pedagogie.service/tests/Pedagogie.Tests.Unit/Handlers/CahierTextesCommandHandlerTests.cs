using FluentAssertions;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Handlers;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour CahierTextesCommandHandler.
/// </summary>
public sealed class CahierTextesCommandHandlerTests
{
    private readonly Mock<ICahierTextesService> _serviceMock;
    private readonly CahierTextesCommandHandler _sut;

    public CahierTextesCommandHandlerTests()
    {
        _serviceMock = new Mock<ICahierTextesService>();
        _sut = new CahierTextesCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneCahierTextes()
    {
        var command = new CreateCahierTextesCommand(
            Name: "Cours du 01/09",
            Description: "Cahier de textes",
            SeanceId: 1,
            Contenu: "Introduction aux equations",
            DateSeance: new DateTime(2025, 9, 1),
            TravailAFaire: "Exercices page 12");

        var expected = new CahierTextes
        {
            Id = 42,
            Name = "Cours du 01/09",
            SeanceId = 1,
            Contenu = "Introduction aux equations"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<CahierTextes>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<CahierTextes>(c =>
                c.Name == "Cours du 01/09" &&
                c.SeanceId == 1 &&
                c.Contenu == "Introduction aux equations"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateCahierTextesCommand(
            Id: 1, Name: "Cours du 01/09", Description: null,
            SeanceId: 1, Contenu: "Contenu mis a jour",
            DateSeance: new DateTime(2025, 9, 1),
            TravailAFaire: null);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CahierTextes>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<CahierTextes>(c => c.Id == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateCahierTextesCommand(
            Id: 999, Name: "X", Description: null,
            SeanceId: 1, Contenu: "X",
            DateSeance: DateTime.Today, TravailAFaire: null);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<CahierTextes>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteCahierTextesCommand(1);

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
        var command = new DeleteCahierTextesCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
