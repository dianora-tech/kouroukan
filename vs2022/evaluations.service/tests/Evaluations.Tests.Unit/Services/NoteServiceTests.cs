using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Output;
using Evaluations.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Evaluations.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour NoteService.
/// </summary>
public sealed class NoteServiceTests
{
    private readonly Mock<INoteRepository> _repoMock;
    private readonly Mock<IEvaluationRepository> _evaluationRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<NoteService>> _loggerMock;
    private readonly NoteService _sut;

    public NoteServiceTests()
    {
        _repoMock = new Mock<INoteRepository>();
        _evaluationRepoMock = new Mock<IEvaluationRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<NoteService>>();

        _sut = new NoteService(
            _repoMock.Object,
            _evaluationRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneNote_QuandExiste()
    {
        var note = CreateNote();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(note);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Note?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var notes = new List<Note>
        {
            CreateNote(),
            CreateNote(id: 2)
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(notes);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Note>(
            new List<Note> { CreateNote() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLaNote_AvecDonneesValides()
    {
        var entity = CreateNote();
        var evaluation = CreateEvaluation();

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _repoMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreateNote();
        var evaluation = CreateEvaluation();

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Note>>(),
            "kouroukan.events",
            "entity.created.note",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiEvaluationInexistante()
    {
        var entity = CreateNote();

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Evaluation?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*evaluation*n'existe pas*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiNoteNegative()
    {
        var entity = CreateNote(valeur: -1);
        var evaluation = CreateEvaluation();

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*note*negative*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiNoteDepasseMaximale()
    {
        var entity = CreateNote(valeur: 25);
        var evaluation = CreateEvaluation(noteMaximale: 20);

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*note*depasser*note maximale*");
    }

    [Fact]
    public async Task CreateAsync_AccepteNoteEgaleAMaximale()
    {
        var entity = CreateNote(valeur: 20);
        var evaluation = CreateEvaluation(noteMaximale: 20);

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_AccepteNoteZero()
    {
        var entity = CreateNote(valeur: 0);
        var evaluation = CreateEvaluation(noteMaximale: 20);

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateNote();
        var evaluation = CreateEvaluation();

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateNote();
        var evaluation = CreateEvaluation();

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Note>>(),
            "kouroukan.events",
            "entity.updated.note",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateNote();
        var evaluation = CreateEvaluation();

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Note>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_KeyNotFoundException_SiEvaluationInexistante()
    {
        var entity = CreateNote();

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Evaluation?)null);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*evaluation*n'existe pas*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiNoteNegative()
    {
        var entity = CreateNote(valeur: -5);
        var evaluation = CreateEvaluation();

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*note*negative*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiNoteDepasseMaximale()
    {
        var entity = CreateNote(valeur: 25);
        var evaluation = CreateEvaluation(noteMaximale: 20);

        _evaluationRepoMock.Setup(r => r.GetByIdAsync(entity.EvaluationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*note*depasser*note maximale*");
    }

    // ─── DeleteAsync ───

    [Fact]
    public async Task DeleteAsync_RetourneTrue_QuandSuppressionReussie()
    {
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.DeleteAsync(1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_PublieEvenement_SiReussite()
    {
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.DeleteAsync(1);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityDeletedEvent<Note>>(),
            "kouroukan.events",
            "entity.deleted.note",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_RetourneFalse_QuandInexistante()
    {
        _repoMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.DeleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_NePubliePas_SiEchec()
    {
        _repoMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.DeleteAsync(999);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityDeletedEvent<Note>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static Note CreateNote(int id = 1, decimal valeur = 15m)
    {
        return new Note
        {
            Id = id,
            EvaluationId = 10,
            EleveId = 5,
            Valeur = valeur,
            Commentaire = "Bon travail",
            DateSaisie = new DateTime(2025, 10, 20),
            UserId = 1
        };
    }

    private static Evaluation CreateEvaluation(decimal noteMaximale = 20m)
    {
        return new Evaluation
        {
            Id = 10,
            TypeId = 1,
            MatiereId = 10,
            ClasseId = 5,
            EnseignantId = 3,
            DateEvaluation = new DateTime(2025, 10, 15),
            Coefficient = 2m,
            NoteMaximale = noteMaximale,
            Trimestre = 1,
            AnneeScolaireId = 1,
            UserId = 1
        };
    }
}
