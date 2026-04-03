using Finances.Application.Commands;
using Finances.Application.Handlers;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour DepenseCommandHandler.
/// </summary>
public sealed class DepenseCommandHandlerTests
{
    private readonly Mock<IDepenseService> _serviceMock;
    private readonly DepenseCommandHandler _sut;

    public DepenseCommandHandlerTests()
    {
        _serviceMock = new Mock<IDepenseService>();
        _sut = new DepenseCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneDepense()
    {
        var command = new CreateDepenseCommand(
            TypeId: 1, Montant: 500000m, MotifDepense: "Achat fournitures",
            Categorie: "Fournitures", BeneficiaireNom: "ABC",
            BeneficiaireTelephone: "622000000", BeneficiaireNIF: null,
            StatutDepense: "Demande", DateDemande: new DateTime(2025, 9, 1),
            DateValidation: null, ValidateurId: null, PieceJointeUrl: null,
            NumeroJustificatif: "DEP-001", UserId: 1);

        var expected = new Depense { Id = 42, Montant = 500000m, Categorie = "Fournitures" };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Depense>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Depense>(e => e.Montant == 500000m && e.Categorie == "Fournitures"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateDepenseCommand(
            Id: 1, TypeId: 1, Montant: 600000m, MotifDepense: "Motif modifie",
            Categorie: "Maintenance", BeneficiaireNom: "XYZ",
            BeneficiaireTelephone: null, BeneficiaireNIF: null,
            StatutDepense: "ValideN1", DateDemande: DateTime.Today,
            DateValidation: DateTime.Today, ValidateurId: 2, PieceJointeUrl: null,
            NumeroJustificatif: "DEP-001", UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Depense>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(new DeleteDepenseCommand(1), CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new DeleteDepenseCommand(999), CancellationToken.None);

        result.Should().BeFalse();
    }
}
