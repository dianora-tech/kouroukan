using FluentAssertions;
using Evaluations.Application.Commands;
using Evaluations.Application.Handlers;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Evaluations.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour BulletinCommandHandler.
/// </summary>
public sealed class BulletinCommandHandlerTests
{
    private readonly Mock<IBulletinService> _serviceMock;
    private readonly BulletinCommandHandler _sut;

    public BulletinCommandHandlerTests()
    {
        _serviceMock = new Mock<IBulletinService>();
        _sut = new BulletinCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneBulletin()
    {
        var command = new CreateBulletinCommand(
            EleveId: 5,
            ClasseId: 3,
            Trimestre: 1,
            AnneeScolaireId: 1,
            MoyenneGenerale: 14.5m,
            Rang: 3,
            Appreciation: "Bon trimestre",
            EstPublie: false,
            DateGeneration: new DateTime(2025, 12, 20),
            CheminFichierPdf: null,
            UserId: 1);

        var expected = new Bulletin
        {
            Id = 42,
            EleveId = 5,
            ClasseId = 3,
            Trimestre = 1,
            MoyenneGenerale = 14.5m,
            Rang = 3
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Bulletin>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Bulletin>(b =>
                b.EleveId == 5 &&
                b.ClasseId == 3 &&
                b.Trimestre == 1 &&
                b.MoyenneGenerale == 14.5m &&
                b.Rang == 3),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateBulletinCommand(
            Id: 1,
            EleveId: 5,
            ClasseId: 3,
            Trimestre: 1,
            AnneeScolaireId: 1,
            MoyenneGenerale: 16m,
            Rang: 1,
            Appreciation: "Excellent trimestre",
            EstPublie: true,
            DateGeneration: new DateTime(2025, 12, 20),
            CheminFichierPdf: null,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Bulletin>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Bulletin>(b => b.Id == 1 && b.MoyenneGenerale == 16m && b.EstPublie),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateBulletinCommand(
            Id: 999, EleveId: 5, ClasseId: 3, Trimestre: 1, AnneeScolaireId: 1,
            MoyenneGenerale: 10m, Rang: null, Appreciation: null, EstPublie: false,
            DateGeneration: DateTime.Today, CheminFichierPdf: null, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Bulletin>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteBulletinCommand(1);

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
        var command = new DeleteBulletinCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
