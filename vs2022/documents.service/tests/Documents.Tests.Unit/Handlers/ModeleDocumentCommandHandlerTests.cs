using FluentAssertions;
using Documents.Application.Commands;
using Documents.Application.Handlers;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Documents.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour ModeleDocumentCommandHandler.
/// </summary>
public sealed class ModeleDocumentCommandHandlerTests
{
    private readonly Mock<IModeleDocumentService> _serviceMock;
    private readonly ModeleDocumentCommandHandler _sut;

    public ModeleDocumentCommandHandlerTests()
    {
        _serviceMock = new Mock<IModeleDocumentService>();
        _sut = new ModeleDocumentCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneModeleDocument()
    {
        var command = new CreateModeleDocumentCommand(
            TypeId: 1,
            Code: "BULL_NOTES",
            Contenu: "<html>{{nom}}</html>",
            LogoUrl: "https://example.com/logo.png",
            CouleurPrimaire: "#16a34a",
            TextePiedPage: "Pied de page",
            EstActif: true,
            UserId: 1);

        var expected = new ModeleDocument
        {
            Id = 42,
            TypeId = 1,
            Code = "BULL_NOTES",
            Contenu = "<html>{{nom}}</html>"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<ModeleDocument>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<ModeleDocument>(e =>
                e.TypeId == 1 &&
                e.Code == "BULL_NOTES" &&
                e.EstActif),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateModeleDocumentCommand(
            Id: 1,
            TypeId: 1,
            Code: "ATT_SCOL",
            Contenu: "<html>updated</html>",
            LogoUrl: null,
            CouleurPrimaire: "#ff0000",
            TextePiedPage: null,
            EstActif: false,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<ModeleDocument>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<ModeleDocument>(e => e.Id == 1 && e.Code == "ATT_SCOL"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateModeleDocumentCommand(
            Id: 999, TypeId: 1, Code: "TEST", Contenu: "c", LogoUrl: null,
            CouleurPrimaire: null, TextePiedPage: null, EstActif: true, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<ModeleDocument>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteModeleDocumentCommand(1);

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
        var command = new DeleteModeleDocumentCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
