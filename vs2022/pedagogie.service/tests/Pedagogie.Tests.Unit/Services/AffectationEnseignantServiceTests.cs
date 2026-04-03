using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Output;
using Pedagogie.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour AffectationEnseignantService.
/// </summary>
public sealed class AffectationEnseignantServiceTests
{
    private readonly Mock<IAffectationEnseignantRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<AffectationEnseignantService>> _loggerMock;
    private readonly AffectationEnseignantService _sut;

    public AffectationEnseignantServiceTests()
    {
        _repoMock = new Mock<IAffectationEnseignantRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<AffectationEnseignantService>>();

        _sut = new AffectationEnseignantService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneAffectation_QuandExiste()
    {
        var affectation = CreateAffectation();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(affectation);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AffectationEnseignant?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<AffectationEnseignant>(
            new List<AffectationEnseignant> { CreateAffectation() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLAffectation_EtRetourneEntite()
    {
        var entity = CreateAffectation();

        _repoMock.Setup(r => r.AddAsync(It.IsAny<AffectationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<AffectationEnseignant>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MetEstActiveATrue()
    {
        var entity = CreateAffectation();
        entity.EstActive = false;

        _repoMock.Setup(r => r.AddAsync(It.IsAny<AffectationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        entity.EstActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreateAffectation();

        _repoMock.Setup(r => r.AddAsync(It.IsAny<AffectationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<AffectationEnseignant>>(),
            "kouroukan.events",
            "entity.created.affectation-enseignant",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateAffectation();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateAffectation();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<AffectationEnseignant>>(),
            "kouroukan.events",
            "entity.updated.affectation-enseignant",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateAffectation();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<AffectationEnseignant>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
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
            It.IsAny<EntityDeletedEvent<AffectationEnseignant>>(),
            "kouroukan.events",
            "entity.deleted.affectation-enseignant",
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

    // ─── Helper ───

    private static AffectationEnseignant CreateAffectation()
    {
        return new AffectationEnseignant
        {
            Id = 1,
            LiaisonId = 10,
            ClasseId = 5,
            MatiereId = 3,
            AnneeScolaireId = 1,
            EstActive = true
        };
    }
}
