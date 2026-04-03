using FluentAssertions;
using Personnel.Application.Commands;
using Personnel.Application.Handlers;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Personnel.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour DemandeCongeCommandHandler.
/// </summary>
public sealed class DemandeCongeCommandHandlerTests
{
    private readonly Mock<IDemandeCongeService> _serviceMock;
    private readonly DemandeCongeCommandHandler _sut;

    public DemandeCongeCommandHandlerTests()
    {
        _serviceMock = new Mock<IDemandeCongeService>();
        _sut = new DemandeCongeCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneDemandeConge()
    {
        var command = new CreateDemandeCongeCommand(
            Name: "Conge annuel",
            Description: "Conge de fin d'annee",
            EnseignantId: 10,
            DateDebut: new DateTime(2025, 7, 1),
            DateFin: new DateTime(2025, 7, 15),
            Motif: "Repos annuel",
            StatutDemande: "Soumise",
            PieceJointeUrl: null,
            CommentaireValidateur: null,
            ValidateurId: null,
            DateValidation: null,
            ImpactPaie: false,
            TypeId: 1,
            UserId: 1);

        var expected = new DemandeConge
        {
            Id = 42,
            Name = "Conge annuel",
            EnseignantId = 10,
            StatutDemande = "Soumise"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<DemandeConge>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<DemandeConge>(d =>
                d.Name == "Conge annuel" &&
                d.EnseignantId == 10 &&
                d.Motif == "Repos annuel" &&
                d.StatutDemande == "Soumise"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateCommand_MappeToutes_LesProprietesVersEntite()
    {
        var dateDebut = new DateTime(2025, 8, 1);
        var dateFin = new DateTime(2025, 8, 10);
        var dateValidation = new DateTime(2025, 7, 25);

        var command = new CreateDemandeCongeCommand(
            Name: "Conge maladie",
            Description: "Certificat medical fourni",
            EnseignantId: 20,
            DateDebut: dateDebut,
            DateFin: dateFin,
            Motif: "Maladie",
            StatutDemande: "ApprouveeN1",
            PieceJointeUrl: "https://docs.example.com/certificat.pdf",
            CommentaireValidateur: "Approuve par le directeur",
            ValidateurId: 5,
            DateValidation: dateValidation,
            ImpactPaie: true,
            TypeId: 2,
            UserId: 3);

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<DemandeConge>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DemandeConge { Id = 1 });

        await _sut.Handle(command, CancellationToken.None);

        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<DemandeConge>(d =>
                d.Name == "Conge maladie" &&
                d.Description == "Certificat medical fourni" &&
                d.EnseignantId == 20 &&
                d.DateDebut == dateDebut &&
                d.DateFin == dateFin &&
                d.Motif == "Maladie" &&
                d.StatutDemande == "ApprouveeN1" &&
                d.PieceJointeUrl == "https://docs.example.com/certificat.pdf" &&
                d.CommentaireValidateur == "Approuve par le directeur" &&
                d.ValidateurId == 5 &&
                d.DateValidation == dateValidation &&
                d.ImpactPaie == true &&
                d.TypeId == 2 &&
                d.UserId == 3),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateDemandeCongeCommand(
            Id: 1,
            Name: "Conge annuel modifie",
            Description: null,
            EnseignantId: 10,
            DateDebut: new DateTime(2025, 7, 1),
            DateFin: new DateTime(2025, 7, 20),
            Motif: "Repos prolonge",
            StatutDemande: "ApprouveeDirection",
            PieceJointeUrl: null,
            CommentaireValidateur: "OK",
            ValidateurId: 5,
            DateValidation: new DateTime(2025, 6, 28),
            ImpactPaie: true,
            TypeId: 1,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<DemandeConge>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<DemandeConge>(d => d.Id == 1 && d.StatutDemande == "ApprouveeDirection"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateDemandeCongeCommand(
            Id: 999, Name: "Test", Description: null, EnseignantId: 10,
            DateDebut: DateTime.Today, DateFin: DateTime.Today.AddDays(5),
            Motif: "Test", StatutDemande: "Soumise", PieceJointeUrl: null,
            CommentaireValidateur: null, ValidateurId: null, DateValidation: null,
            ImpactPaie: false, TypeId: 1, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<DemandeConge>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteDemandeCongeCommand(1);

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
        var command = new DeleteDemandeCongeCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
