using FluentAssertions;
using GnDapper.Models;
using Evaluations.Application.Handlers;
using Evaluations.Application.Queries;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Evaluations.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour NoteQueryHandler.
/// </summary>
public sealed class NoteQueryHandlerTests
{
    private readonly Mock<INoteService> _serviceMock;
    private readonly NoteQueryHandler _sut;

    public NoteQueryHandlerTests()
    {
        _serviceMock = new Mock<INoteService>();
        _sut = new NoteQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetById_RetourneNote()
    {
        var note = new Note { Id = 1, EvaluationId = 10, EleveId = 5, Valeur = 15m };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(note);

        var result = await _sut.Handle(new GetNoteByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Note?)null);

        var result = await _sut.Handle(new GetNoteByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var notes = new List<Note>
        {
            new() { Id = 1, Valeur = 15m },
            new() { Id = 2, Valeur = 12m },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(notes);

        var result = await _sut.Handle(new GetAllNotesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Note>(
            new List<Note> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedNotesQuery(1, 20, "test", "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
