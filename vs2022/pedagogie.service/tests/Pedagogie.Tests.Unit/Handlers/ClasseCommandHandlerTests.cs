using FluentAssertions;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Handlers;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour ClasseCommandHandler.
/// </summary>
public sealed class ClasseCommandHandlerTests
{
    private readonly Mock<IClasseService> _serviceMock;
    private readonly ClasseCommandHandler _sut;

    public ClasseCommandHandlerTests()
    {
        _serviceMock = new Mock<IClasseService>();
        _sut = new ClasseCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneClasse()
    {
        var command = new CreateClasseCommand(
            Name: "7eme A",
            Description: "Classe de 7eme",
            NiveauClasseId: 1,
            Capacite: 40,
            AnneeScolaireId: 1,
            EnseignantPrincipalId: null,
            Effectif: 30);

        var expected = new Classe
        {
            Id = 42,
            Name = "7eme A",
            NiveauClasseId = 1,
            Capacite = 40
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Classe>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Classe>(c =>
                c.Name == "7eme A" &&
                c.NiveauClasseId == 1 &&
                c.Capacite == 40 &&
                c.Effectif == 30),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateClasseCommand(
            Id: 1, Name: "7eme A", Description: null,
            NiveauClasseId: 1, Capacite: 40, AnneeScolaireId: 1,
            EnseignantPrincipalId: null, Effectif: 30);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Classe>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Classe>(c => c.Id == 1 && c.Name == "7eme A"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateClasseCommand(
            Id: 999, Name: "X", Description: null,
            NiveauClasseId: 1, Capacite: 40, AnneeScolaireId: 1,
            EnseignantPrincipalId: null, Effectif: 0);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Classe>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteClasseCommand(1);

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
        var command = new DeleteClasseCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
