using FluentAssertions;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Handlers;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AnneeScolaireCommandHandler.
/// </summary>
public sealed class AnneeScolaireCommandHandlerTests
{
    private readonly Mock<IAnneeScolaireService> _serviceMock;
    private readonly AnneeScolaireCommandHandler _sut;

    public AnneeScolaireCommandHandlerTests()
    {
        _serviceMock = new Mock<IAnneeScolaireService>();
        _sut = new AnneeScolaireCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneAnneeScolaire()
    {
        var command = new CreateAnneeScolaireCommand(
            Libelle: "2025-2026",
            DateDebut: new DateTime(2025, 10, 1),
            DateFin: new DateTime(2026, 6, 30),
            EstActive: true,
            Code: "2025-2026",
            Description: "Annee scolaire 2025-2026",
            Statut: "preparation",
            DateRentree: new DateTime(2025, 10, 1),
            NombrePeriodes: 3,
            TypePeriode: "trimestre");

        var expected = new AnneeScolaire
        {
            Id = 42,
            Libelle = "2025-2026",
            EstActive = true
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<AnneeScolaire>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<AnneeScolaire>(a =>
                a.Libelle == "2025-2026" &&
                a.EstActive &&
                a.NombrePeriodes == 3 &&
                a.TypePeriode == "trimestre"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateCommand_MappeCorrectementTousLesChamps()
    {
        var command = new CreateAnneeScolaireCommand(
            Libelle: "2024-2025",
            DateDebut: new DateTime(2024, 10, 1),
            DateFin: new DateTime(2025, 6, 30),
            EstActive: false,
            Code: "AS-2024",
            Description: "Description test",
            Statut: "active",
            DateRentree: new DateTime(2024, 10, 7),
            NombrePeriodes: 2,
            TypePeriode: "semestre");

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<AnneeScolaire>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnneeScolaire { Id = 1 });

        await _sut.Handle(command, CancellationToken.None);

        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<AnneeScolaire>(a =>
                a.Code == "AS-2024" &&
                a.Description == "Description test" &&
                a.Statut == "active" &&
                a.NombrePeriodes == 2 &&
                a.TypePeriode == "semestre" &&
                a.DateRentree == new DateTime(2024, 10, 7)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateAnneeScolaireCommand(
            Id: 1,
            Libelle: "2025-2026",
            DateDebut: new DateTime(2025, 10, 1),
            DateFin: new DateTime(2026, 6, 30),
            EstActive: true,
            Code: "2025-2026",
            Description: null,
            Statut: "active",
            DateRentree: null,
            NombrePeriodes: 3,
            TypePeriode: "trimestre");

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<AnneeScolaire>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<AnneeScolaire>(a => a.Id == 1 && a.Libelle == "2025-2026"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateAnneeScolaireCommand(
            Id: 999,
            Libelle: "X",
            DateDebut: DateTime.Today,
            DateFin: DateTime.Today.AddMonths(9),
            EstActive: false);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<AnneeScolaire>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteAnneeScolaireCommand(1);

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
        var command = new DeleteAnneeScolaireCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
