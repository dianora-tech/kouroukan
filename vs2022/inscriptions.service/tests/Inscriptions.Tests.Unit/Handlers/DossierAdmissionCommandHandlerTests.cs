using FluentAssertions;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Handlers;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour DossierAdmissionCommandHandler.
/// </summary>
public sealed class DossierAdmissionCommandHandlerTests
{
    private readonly Mock<IDossierAdmissionService> _serviceMock;
    private readonly DossierAdmissionCommandHandler _sut;

    public DossierAdmissionCommandHandlerTests()
    {
        _serviceMock = new Mock<IDossierAdmissionService>();
        _sut = new DossierAdmissionCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneDossier()
    {
        var command = new CreateDossierAdmissionCommand(
            TypeId: 1,
            EleveId: 10,
            AnneeScolaireId: 1,
            StatutDossier: "EnEtude",
            EtapeActuelle: "DepotDossier",
            DateDemande: new DateTime(2025, 6, 1),
            DateDecision: null,
            MotifRefus: null,
            ScoringInterne: null,
            Commentaires: null,
            ResponsableAdmissionId: null,
            UserId: 1);

        var expected = new DossierAdmission
        {
            Id = 42,
            TypeId = 1,
            EleveId = 10,
            StatutDossier = "EnEtude"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<DossierAdmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<DossierAdmission>(d =>
                d.TypeId == 1 &&
                d.EleveId == 10 &&
                d.StatutDossier == "EnEtude" &&
                d.EtapeActuelle == "DepotDossier"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateCommand_MappeCorrectementTousLesChamps()
    {
        var command = new CreateDossierAdmissionCommand(
            TypeId: 2,
            EleveId: 20,
            AnneeScolaireId: 3,
            StatutDossier: "Admis",
            EtapeActuelle: "Decision",
            DateDemande: new DateTime(2025, 6, 1),
            DateDecision: new DateTime(2025, 7, 1),
            MotifRefus: null,
            ScoringInterne: 85.5m,
            Commentaires: "Bon dossier",
            ResponsableAdmissionId: 5,
            UserId: 2);

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<DossierAdmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DossierAdmission { Id = 1 });

        await _sut.Handle(command, CancellationToken.None);

        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<DossierAdmission>(d =>
                d.AnneeScolaireId == 3 &&
                d.ScoringInterne == 85.5m &&
                d.Commentaires == "Bon dossier" &&
                d.ResponsableAdmissionId == 5 &&
                d.UserId == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateDossierAdmissionCommand(
            Id: 1,
            TypeId: 1,
            EleveId: 10,
            AnneeScolaireId: 1,
            StatutDossier: "Admis",
            EtapeActuelle: "Decision",
            DateDemande: new DateTime(2025, 6, 1),
            DateDecision: new DateTime(2025, 7, 15),
            MotifRefus: null,
            ScoringInterne: 90m,
            Commentaires: "Excellent",
            ResponsableAdmissionId: 5,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<DossierAdmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<DossierAdmission>(d => d.Id == 1 && d.StatutDossier == "Admis"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateDossierAdmissionCommand(
            Id: 999, TypeId: 1, EleveId: 10, AnneeScolaireId: 1,
            StatutDossier: "EnEtude", EtapeActuelle: "DepotDossier",
            DateDemande: DateTime.Today, DateDecision: null, MotifRefus: null,
            ScoringInterne: null, Commentaires: null, ResponsableAdmissionId: null, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<DossierAdmission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteDossierAdmissionCommand(1);

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
        var command = new DeleteDossierAdmissionCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
