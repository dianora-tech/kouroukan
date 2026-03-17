using FluentAssertions;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Handlers;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour InscriptionCommandHandler.
/// </summary>
public sealed class InscriptionCommandHandlerTests
{
    private readonly Mock<IInscriptionService> _serviceMock;
    private readonly InscriptionCommandHandler _sut;

    public InscriptionCommandHandlerTests()
    {
        _serviceMock = new Mock<IInscriptionService>();
        _sut = new InscriptionCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneInscription()
    {
        var command = new CreateInscriptionCommand(
            TypeId: 1,
            EleveId: 10,
            ClasseId: 5,
            AnneeScolaireId: 1,
            DateInscription: new DateTime(2025, 9, 1),
            MontantInscription: 150000m,
            EstPaye: false,
            EstRedoublant: false,
            TypeEtablissement: "Public",
            SerieBac: null,
            StatutInscription: "EnAttente",
            UserId: 1);

        var expected = new Inscription
        {
            Id = 42,
            TypeId = 1,
            EleveId = 10,
            StatutInscription = "EnAttente"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Inscription>(i =>
                i.TypeId == 1 &&
                i.EleveId == 10 &&
                i.ClasseId == 5 &&
                i.StatutInscription == "EnAttente"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateInscriptionCommand(
            Id: 1,
            TypeId: 1,
            EleveId: 10,
            ClasseId: 5,
            AnneeScolaireId: 1,
            DateInscription: new DateTime(2025, 9, 1),
            MontantInscription: 200000m,
            EstPaye: true,
            EstRedoublant: false,
            TypeEtablissement: null,
            SerieBac: null,
            StatutInscription: "Validee",
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Inscription>(i => i.Id == 1 && i.EstPaye),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateInscriptionCommand(
            Id: 999, TypeId: 1, EleveId: 10, ClasseId: 5, AnneeScolaireId: 1,
            DateInscription: DateTime.Today, MontantInscription: 0, EstPaye: false,
            EstRedoublant: false, TypeEtablissement: null, SerieBac: null,
            StatutInscription: "EnAttente", UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteInscriptionCommand(1);

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
        var command = new DeleteInscriptionCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
