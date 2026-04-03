using FluentAssertions;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Handlers;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour EleveCommandHandler.
/// </summary>
public sealed class EleveCommandHandlerTests
{
    private readonly Mock<IEleveService> _serviceMock;
    private readonly EleveCommandHandler _sut;

    public EleveCommandHandlerTests()
    {
        _serviceMock = new Mock<IEleveService>();
        _sut = new EleveCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneEleve()
    {
        var command = new CreateEleveCommand(
            FirstName: "Mamadou",
            LastName: "Diallo",
            DateNaissance: new DateTime(2010, 5, 15),
            LieuNaissance: "Conakry",
            Genre: "M",
            Nationalite: "Guineenne",
            Adresse: "Kaloum",
            PhotoUrl: null,
            NumeroMatricule: "MAT-001",
            NiveauClasseId: 1,
            ClasseId: 2,
            ParentId: 5,
            StatutInscription: "Inscrit",
            UserId: 1);

        var expected = new Eleve
        {
            Id = 42,
            FirstName = "Mamadou",
            LastName = "Diallo",
            Genre = "M",
            StatutInscription = "Inscrit"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Eleve>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Eleve>(e =>
                e.FirstName == "Mamadou" &&
                e.LastName == "Diallo" &&
                e.Genre == "M" &&
                e.NiveauClasseId == 1 &&
                e.NumeroMatricule == "MAT-001"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateCommand_MappeCorrectementTousLesChamps()
    {
        var command = new CreateEleveCommand(
            FirstName: "Aissatou",
            LastName: "Bah",
            DateNaissance: new DateTime(2012, 3, 20),
            LieuNaissance: "Labe",
            Genre: "F",
            Nationalite: "Guineenne",
            Adresse: "Centre-ville",
            PhotoUrl: "https://example.com/photo.jpg",
            NumeroMatricule: "MAT-002",
            NiveauClasseId: 3,
            ClasseId: 4,
            ParentId: 10,
            StatutInscription: "PreInscrit",
            UserId: 2);

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Eleve>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Eleve { Id = 1 });

        await _sut.Handle(command, CancellationToken.None);

        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Eleve>(e =>
                e.FirstName == "Aissatou" &&
                e.LastName == "Bah" &&
                e.LieuNaissance == "Labe" &&
                e.Nationalite == "Guineenne" &&
                e.Adresse == "Centre-ville" &&
                e.PhotoUrl == "https://example.com/photo.jpg" &&
                e.ClasseId == 4 &&
                e.ParentId == 10 &&
                e.UserId == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateEleveCommand(
            Id: 1,
            FirstName: "Mamadou",
            LastName: "Diallo",
            DateNaissance: new DateTime(2010, 5, 15),
            LieuNaissance: "Conakry",
            Genre: "M",
            Nationalite: "Guineenne",
            Adresse: null,
            PhotoUrl: null,
            NumeroMatricule: "MAT-001",
            NiveauClasseId: 1,
            ClasseId: null,
            ParentId: null,
            StatutInscription: "Inscrit",
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Eleve>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Eleve>(e => e.Id == 1 && e.FirstName == "Mamadou"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateEleveCommand(
            Id: 999, FirstName: "X", LastName: "Y",
            DateNaissance: DateTime.Today, LieuNaissance: "Z",
            Genre: "M", Nationalite: "N", Adresse: null, PhotoUrl: null,
            NumeroMatricule: "MAT-999", NiveauClasseId: 1, ClasseId: null,
            ParentId: null, StatutInscription: "Inscrit", UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Eleve>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteEleveCommand(1);

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
        var command = new DeleteEleveCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
