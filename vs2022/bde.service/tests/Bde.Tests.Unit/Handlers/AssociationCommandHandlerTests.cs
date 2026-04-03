using FluentAssertions;
using Bde.Application.Commands;
using Bde.Application.Handlers;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Bde.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AssociationCommandHandler.
/// </summary>
public sealed class AssociationCommandHandlerTests
{
    private readonly Mock<IAssociationService> _serviceMock;
    private readonly AssociationCommandHandler _sut;

    public AssociationCommandHandlerTests()
    {
        _serviceMock = new Mock<IAssociationService>();
        _sut = new AssociationCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneAssociation()
    {
        var command = new CreateAssociationCommand(
            TypeId: 1,
            Name: "BDE Informatique",
            Description: "Association etudiante",
            Sigle: "BDE",
            AnneeScolaire: "2025-2026",
            Statut: "Active",
            BudgetAnnuel: 500000m,
            SuperviseurId: 10,
            UserId: 1);

        var expected = new Association
        {
            Id = 42,
            TypeId = 1,
            Name = "BDE Informatique",
            Statut = "Active"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Association>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Association>(a =>
                a.TypeId == 1 &&
                a.Name == "BDE Informatique" &&
                a.Statut == "Active" &&
                a.BudgetAnnuel == 500000m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateAssociationCommand(
            Id: 1,
            TypeId: 1,
            Name: "BDE Informatique Maj",
            Description: "Mise a jour",
            Sigle: "BDE",
            AnneeScolaire: "2025-2026",
            Statut: "Suspendue",
            BudgetAnnuel: 600000m,
            SuperviseurId: 10,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Association>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Association>(a => a.Id == 1 && a.Statut == "Suspendue"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateAssociationCommand(
            Id: 999, TypeId: 1, Name: "Test", Description: null, Sigle: null,
            AnneeScolaire: "2025-2026", Statut: "Active", BudgetAnnuel: 0,
            SuperviseurId: null, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Association>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteAssociationCommand(1);

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
        var command = new DeleteAssociationCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
