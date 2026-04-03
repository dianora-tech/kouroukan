using FluentAssertions;
using Documents.Application.Commands;
using Documents.Application.Handlers;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Documents.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour DocumentGenereCommandHandler.
/// </summary>
public sealed class DocumentGenereCommandHandlerTests
{
    private readonly Mock<IDocumentGenereService> _serviceMock;
    private readonly DocumentGenereCommandHandler _sut;

    public DocumentGenereCommandHandlerTests()
    {
        _serviceMock = new Mock<IDocumentGenereService>();
        _sut = new DocumentGenereCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneDocumentGenere()
    {
        var command = new CreateDocumentGenereCommand(
            TypeId: 1,
            ModeleDocumentId: 10,
            EleveId: 5,
            EnseignantId: null,
            DonneesJson: "{\"nom\":\"Test\"}",
            DateGeneration: new DateTime(2025, 6, 15),
            StatutSignature: "EnAttente",
            CheminFichier: "/documents/test.pdf",
            UserId: 1);

        var expected = new DocumentGenere
        {
            Id = 42,
            TypeId = 1,
            ModeleDocumentId = 10,
            StatutSignature = "EnAttente"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<DocumentGenere>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<DocumentGenere>(e =>
                e.TypeId == 1 &&
                e.ModeleDocumentId == 10 &&
                e.EleveId == 5 &&
                e.StatutSignature == "EnAttente"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateDocumentGenereCommand(
            Id: 1,
            TypeId: 1,
            ModeleDocumentId: 10,
            EleveId: 5,
            EnseignantId: null,
            DonneesJson: "{\"nom\":\"Test\"}",
            DateGeneration: new DateTime(2025, 6, 15),
            StatutSignature: "Signe",
            CheminFichier: "/documents/test.pdf",
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<DocumentGenere>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<DocumentGenere>(e => e.Id == 1 && e.StatutSignature == "Signe"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateDocumentGenereCommand(
            Id: 999, TypeId: 1, ModeleDocumentId: 10, EleveId: null, EnseignantId: null,
            DonneesJson: "{}", DateGeneration: DateTime.Today, StatutSignature: "EnAttente",
            CheminFichier: null, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<DocumentGenere>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteDocumentGenereCommand(1);

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
        var command = new DeleteDocumentGenereCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
