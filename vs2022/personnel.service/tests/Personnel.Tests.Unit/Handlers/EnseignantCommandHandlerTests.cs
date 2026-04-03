using FluentAssertions;
using Personnel.Application.Commands;
using Personnel.Application.Handlers;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Personnel.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour EnseignantCommandHandler.
/// </summary>
public sealed class EnseignantCommandHandlerTests
{
    private readonly Mock<IEnseignantService> _serviceMock;
    private readonly EnseignantCommandHandler _sut;

    public EnseignantCommandHandlerTests()
    {
        _serviceMock = new Mock<IEnseignantService>();
        _sut = new EnseignantCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneEnseignant()
    {
        var command = new CreateEnseignantCommand(
            Name: "Mamadou Diallo",
            Description: "Professeur de mathematiques",
            Matricule: "MAT-001",
            Specialite: "Mathematiques",
            DateEmbauche: new DateTime(2020, 9, 1),
            ModeRemuneration: "Forfait",
            MontantForfait: 500000m,
            Telephone: "+224 620 00 00 00",
            Email: "mamadou.diallo@test.com",
            StatutEnseignant: "Actif",
            SoldeCongesAnnuel: 30,
            TypeId: 1,
            UserId: 1);

        var expected = new Enseignant
        {
            Id = 42,
            Name = "Mamadou Diallo",
            Matricule = "MAT-001",
            StatutEnseignant = "Actif"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Enseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Enseignant>(e =>
                e.Name == "Mamadou Diallo" &&
                e.Matricule == "MAT-001" &&
                e.ModeRemuneration == "Forfait" &&
                e.TypeId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateCommand_MappeToutes_LesProprietesVersEntite()
    {
        var command = new CreateEnseignantCommand(
            Name: "Fatoumata Camara",
            Description: "Professeur de francais",
            Matricule: "MAT-002",
            Specialite: "Francais",
            DateEmbauche: new DateTime(2021, 10, 15),
            ModeRemuneration: "Heures",
            MontantForfait: null,
            Telephone: "+224 621 11 11 11",
            Email: null,
            StatutEnseignant: "EnConge",
            SoldeCongesAnnuel: 15,
            TypeId: 2,
            UserId: 3);

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Enseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Enseignant { Id = 1 });

        await _sut.Handle(command, CancellationToken.None);

        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Enseignant>(e =>
                e.Name == "Fatoumata Camara" &&
                e.Description == "Professeur de francais" &&
                e.Matricule == "MAT-002" &&
                e.Specialite == "Francais" &&
                e.DateEmbauche == new DateTime(2021, 10, 15) &&
                e.ModeRemuneration == "Heures" &&
                e.MontantForfait == null &&
                e.Telephone == "+224 621 11 11 11" &&
                e.Email == null &&
                e.StatutEnseignant == "EnConge" &&
                e.SoldeCongesAnnuel == 15 &&
                e.TypeId == 2 &&
                e.UserId == 3),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateEnseignantCommand(
            Id: 1,
            Name: "Mamadou Diallo",
            Description: "Professeur principal",
            Matricule: "MAT-001",
            Specialite: "Mathematiques",
            DateEmbauche: new DateTime(2020, 9, 1),
            ModeRemuneration: "Mixte",
            MontantForfait: 600000m,
            Telephone: "+224 620 00 00 00",
            Email: "mamadou.diallo@test.com",
            StatutEnseignant: "Actif",
            SoldeCongesAnnuel: 25,
            TypeId: 1,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Enseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Enseignant>(e => e.Id == 1 && e.ModeRemuneration == "Mixte"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateEnseignantCommand(
            Id: 999, Name: "Test", Description: null, Matricule: "MAT-999",
            Specialite: "Test", DateEmbauche: DateTime.Today,
            ModeRemuneration: "Forfait", MontantForfait: null,
            Telephone: "+224 000", Email: null, StatutEnseignant: "Actif",
            SoldeCongesAnnuel: 0, TypeId: 1, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Enseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteEnseignantCommand(1);

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
        var command = new DeleteEnseignantCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
