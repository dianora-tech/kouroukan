using FluentAssertions;
using Communication.Application.Commands;
using Communication.Application.Handlers;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Communication.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AnnonceCommandHandler.
/// </summary>
public sealed class AnnonceCommandHandlerTests
{
    private readonly Mock<IAnnonceService> _serviceMock;
    private readonly AnnonceCommandHandler _sut;

    public AnnonceCommandHandlerTests()
    {
        _serviceMock = new Mock<IAnnonceService>();
        _sut = new AnnonceCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneAnnonce()
    {
        var command = new CreateAnnonceCommand(
            Name: "Annonce test",
            TypeId: 1,
            Contenu: "Contenu de test",
            DateDebut: new DateTime(2025, 9, 1),
            DateFin: new DateTime(2025, 12, 31),
            EstActive: true,
            CibleAudience: "Tous",
            Priorite: 1,
            UserId: 1);

        var expected = new Annonce
        {
            Id = 42,
            Name = "Annonce test",
            TypeId = 1,
            Contenu = "Contenu de test",
            CibleAudience = "Tous"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Annonce>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Annonce>(a =>
                a.Name == "Annonce test" &&
                a.TypeId == 1 &&
                a.Contenu == "Contenu de test" &&
                a.CibleAudience == "Tous" &&
                a.Priorite == 1 &&
                a.UserId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateAnnonceCommand(
            Id: 1,
            Name: "Annonce modifiee",
            TypeId: 1,
            Contenu: "Contenu modifie",
            DateDebut: new DateTime(2025, 9, 1),
            DateFin: null,
            EstActive: true,
            CibleAudience: "Parents",
            Priorite: 2,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Annonce>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Annonce>(a => a.Id == 1 && a.Name == "Annonce modifiee"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateAnnonceCommand(
            Id: 999, Name: "Test", TypeId: 1, Contenu: "Test",
            DateDebut: DateTime.Today, DateFin: null, EstActive: true,
            CibleAudience: "Tous", Priorite: 1, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Annonce>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteAnnonceCommand(1);

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
        var command = new DeleteAnnonceCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
