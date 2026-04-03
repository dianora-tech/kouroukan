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
/// Tests unitaires pour SalleService.
/// </summary>
public sealed class SalleServiceTests
{
    private readonly Mock<ISalleRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<SalleService>> _loggerMock;
    private readonly SalleService _sut;

    public SalleServiceTests()
    {
        _repoMock = new Mock<ISalleRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<SalleService>>();

        _sut = new SalleService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneSalle_QuandExiste()
    {
        var salle = CreateSalle();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(salle);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Salle?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─���─ GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Salle>(
            new List<Salle> { CreateSalle() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLaSalle_EtRetourneEntite()
    {
        var entity = CreateSalle();

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
        var entity = CreateSalle();

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Salle>>(),
            "kouroukan.events",
            "entity.created.salle",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCapaciteZero()
    {
        var entity = CreateSalle();
        entity.Capacite = 0;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*capacite*");
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateSalle();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateSalle();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Salle>>(),
            "kouroukan.events",
            "entity.updated.salle",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateSalle();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Salle>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiCapaciteZero()
    {
        var entity = CreateSalle();
        entity.Capacite = 0;

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*capacite*");
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
            It.IsAny<EntityDeletedEvent<Salle>>(),
            "kouroukan.events",
            "entity.deleted.salle",
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

    private static Salle CreateSalle()
    {
        return new Salle
        {
            Id = 1,
            Name = "Salle 101",
            Description = "Salle de cours principale",
            TypeId = 1,
            Capacite = 40,
            Batiment = "Batiment A",
            Equipements = "Tableau, projecteur",
            EstDisponible = true,
            UserId = 1
        };
    }
}
