using FluentAssertions;
using Bde.Application.Commands;
using Bde.Application.Handlers;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Bde.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour DepenseBdeCommandHandler.
/// </summary>
public sealed class DepenseBdeCommandHandlerTests
{
    private readonly Mock<IDepenseBdeService> _serviceMock;
    private readonly DepenseBdeCommandHandler _sut;

    public DepenseBdeCommandHandlerTests()
    {
        _serviceMock = new Mock<IDepenseBdeService>();
        _sut = new DepenseBdeCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneDepense()
    {
        var command = new CreateDepenseBdeCommand(
            TypeId: 1,
            Name: "Achat materiel",
            Description: "Fournitures",
            AssociationId: 5,
            Montant: 100000m,
            Motif: "Evenement rentree",
            Categorie: "Materiel",
            StatutValidation: "Demandee",
            ValidateurId: 10,
            UserId: 1);

        var expected = new DepenseBde
        {
            Id = 42,
            TypeId = 1,
            Name = "Achat materiel",
            Montant = 100000m
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<DepenseBde>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<DepenseBde>(d =>
                d.TypeId == 1 &&
                d.AssociationId == 5 &&
                d.Montant == 100000m &&
                d.Categorie == "Materiel"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateDepenseBdeCommand(
            Id: 1, TypeId: 1, Name: "Achat maj", Description: null,
            AssociationId: 5, Montant: 200000m, Motif: "Motif maj",
            Categorie: "Location", StatutValidation: "ValideTresorier",
            ValidateurId: 10, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<DepenseBde>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<DepenseBde>(d => d.Id == 1 && d.Montant == 200000m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateDepenseBdeCommand(
            Id: 999, TypeId: 1, Name: "Test", Description: null,
            AssociationId: 5, Montant: 1000m, Motif: "Motif",
            Categorie: "Materiel", StatutValidation: "Demandee",
            ValidateurId: null, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<DepenseBde>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteDepenseBdeCommand(1);

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
        var command = new DeleteDepenseBdeCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
