using FluentAssertions;
using Evaluations.Application.Commands;
using Evaluations.Application.Handlers;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Evaluations.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour NoteCommandHandler.
/// </summary>
public sealed class NoteCommandHandlerTests
{
    private readonly Mock<INoteService> _serviceMock;
    private readonly NoteCommandHandler _sut;

    public NoteCommandHandlerTests()
    {
        _serviceMock = new Mock<INoteService>();
        _sut = new NoteCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneNote()
    {
        var command = new CreateNoteCommand(
            EvaluationId: 10,
            EleveId: 5,
            Valeur: 15.5m,
            Commentaire: "Bon travail",
            DateSaisie: new DateTime(2025, 10, 20),
            UserId: 1);

        var expected = new Note
        {
            Id = 42,
            EvaluationId = 10,
            EleveId = 5,
            Valeur = 15.5m,
            Commentaire = "Bon travail"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Note>(n =>
                n.EvaluationId == 10 &&
                n.EleveId == 5 &&
                n.Valeur == 15.5m &&
                n.Commentaire == "Bon travail"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateNoteCommand(
            Id: 1,
            EvaluationId: 10,
            EleveId: 5,
            Valeur: 18m,
            Commentaire: "Excellent",
            DateSaisie: new DateTime(2025, 10, 20),
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Note>(n => n.Id == 1 && n.Valeur == 18m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateNoteCommand(
            Id: 999, EvaluationId: 10, EleveId: 5, Valeur: 10m,
            Commentaire: null, DateSaisie: DateTime.Today, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteNoteCommand(1);

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
        var command = new DeleteNoteCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
