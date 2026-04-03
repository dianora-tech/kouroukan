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
/// Tests unitaires pour EvaluationService.
/// </summary>
public sealed class EvaluationServiceTests
{
    private readonly Mock<IEvaluationRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<EvaluationService>> _loggerMock;
    private readonly EvaluationService _sut;

    public EvaluationServiceTests()
    {
        _repoMock = new Mock<IEvaluationRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<EvaluationService>>();

        _sut = new EvaluationService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneEvaluation_QuandExiste()
    {
        var evaluation = CreateEvaluation();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluation);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Evaluation?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var evaluations = new List<Evaluation>
        {
            CreateEvaluation(),
            CreateEvaluation(id: 2)
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluations);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Evaluation>(
            new List<Evaluation> { CreateEvaluation() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLEvaluation_AvecDonneesValides()
    {
        var entity = CreateEvaluation();

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
        var entity = CreateEvaluation();

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Evaluation>>(),
            "kouroukan.events",
            "entity.created.evaluation",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task CreateAsync_Lance_InvalidOperationException_SiTrimestreInvalide(int trimestre)
    {
        var entity = CreateEvaluation(trimestre: trimestre);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*trimestre*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiNoteMaximaleZeroOuNegative()
    {
        var entity = CreateEvaluation(noteMaximale: 0);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*note maximale*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCoefficientZeroOuNegatif()
    {
        var entity = CreateEvaluation(coefficient: 0);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*coefficient*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CreateAsync_AccepteTousLesTrimestresValides(int trimestre)
    {
        var entity = CreateEvaluation(trimestre: trimestre);

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateEvaluation();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateEvaluation();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Evaluation>>(),
            "kouroukan.events",
            "entity.updated.evaluation",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateEvaluation();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Evaluation>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiTrimestreInvalide(int trimestre)
    {
        var entity = CreateEvaluation(trimestre: trimestre);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*trimestre*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiNoteMaximaleInvalide()
    {
        var entity = CreateEvaluation(noteMaximale: -5);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*note maximale*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiCoefficientInvalide()
    {
        var entity = CreateEvaluation(coefficient: -1);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*coefficient*");
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
            It.IsAny<EntityDeletedEvent<Evaluation>>(),
            "kouroukan.events",
            "entity.deleted.evaluation",
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
            It.IsAny<EntityDeletedEvent<Evaluation>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static Evaluation CreateEvaluation(
        int id = 1,
        int trimestre = 1,
        decimal noteMaximale = 20m,
        decimal coefficient = 2m)
    {
        return new Evaluation
        {
            Id = id,
            TypeId = 1,
            MatiereId = 10,
            ClasseId = 5,
            EnseignantId = 3,
            DateEvaluation = new DateTime(2025, 10, 15),
            Coefficient = coefficient,
            NoteMaximale = noteMaximale,
            Trimestre = trimestre,
            AnneeScolaireId = 1,
            UserId = 1
        };
    }
}
