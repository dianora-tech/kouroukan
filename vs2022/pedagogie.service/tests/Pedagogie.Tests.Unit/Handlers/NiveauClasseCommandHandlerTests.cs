using FluentAssertions;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Handlers;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour NiveauClasseCommandHandler.
/// </summary>
public sealed class NiveauClasseCommandHandlerTests
{
    private readonly Mock<INiveauClasseService> _serviceMock;
    private readonly NiveauClasseCommandHandler _sut;

    public NiveauClasseCommandHandlerTests()
    {
        _serviceMock = new Mock<INiveauClasseService>();
        _sut = new NiveauClasseCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneNiveauClasse()
    {
        var command = new CreateNiveauClasseCommand(
            Name: "7eme annee",
            Description: "Niveau 7eme",
            TypeId: 1,
            Code: "7E",
            Ordre: 7,
            CycleEtude: "College",
            AgeOfficielEntree: 13,
            MinistereTutelle: "MENA",
            ExamenSortie: null,
            TauxHoraireEnseignant: null);

        var expected = new NiveauClasse
        {
            Id = 42,
            Name = "7eme annee",
            Code = "7E",
            Ordre = 7
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<NiveauClasse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<NiveauClasse>(n =>
                n.Name == "7eme annee" &&
                n.Code == "7E" &&
                n.Ordre == 7 &&
                n.CycleEtude == "College"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateNiveauClasseCommand(
            Id: 1, Name: "7eme annee", Description: null,
            TypeId: 1, Code: "7E", Ordre: 7, CycleEtude: "College",
            AgeOfficielEntree: 13, MinistereTutelle: "MENA",
            ExamenSortie: null, TauxHoraireEnseignant: null);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<NiveauClasse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<NiveauClasse>(n => n.Id == 1 && n.Code == "7E"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateNiveauClasseCommand(
            Id: 999, Name: "X", Description: null,
            TypeId: 1, Code: "X", Ordre: 1, CycleEtude: "College",
            AgeOfficielEntree: null, MinistereTutelle: null,
            ExamenSortie: null, TauxHoraireEnseignant: null);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<NiveauClasse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteNiveauClasseCommand(1);

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
        var command = new DeleteNiveauClasseCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
