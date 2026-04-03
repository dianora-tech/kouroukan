using Finances.Domain.Entities;
using Finances.Domain.Ports.Output;
using Finances.Domain.Services;
using FluentAssertions;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour PalierFamilialService.
/// </summary>
public sealed class PalierFamilialServiceTests
{
    private readonly Mock<IPalierFamilialRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<PalierFamilialService>> _loggerMock;
    private readonly PalierFamilialService _sut;

    public PalierFamilialServiceTests()
    {
        _repoMock = new Mock<IPalierFamilialRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<PalierFamilialService>>();

        _sut = new PalierFamilialService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_RetournePalier_QuandExiste()
    {
        var entity = CreatePalier();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_CreeLePalier_AvecDonneesValides()
    {
        var entity = CreatePalier();
        _repoMock.Setup(r => r.AddAsync(It.IsAny<PalierFamilial>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PalierFamilial e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<PalierFamilial>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreatePalier();
        _repoMock.Setup(r => r.AddAsync(It.IsAny<PalierFamilial>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PalierFamilial e, CancellationToken _) => e);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<PalierFamilial>>(),
            "kouroukan.events",
            "entity.created.palier-familial",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiRangZeroOuNegatif()
    {
        var entity = CreatePalier();
        entity.RangEnfant = 0;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*rang*superieur a 0*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiReductionNegative()
    {
        var entity = CreatePalier();
        entity.ReductionPourcent = -1m;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*pourcentage de reduction*entre 0 et 100*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiReductionSuperieure100()
    {
        var entity = CreatePalier();
        entity.ReductionPourcent = 101m;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*pourcentage de reduction*entre 0 et 100*");
    }

    [Fact]
    public async Task CreateAsync_Accepte_ReductionZero()
    {
        var entity = CreatePalier();
        entity.ReductionPourcent = 0m;

        _repoMock.Setup(r => r.AddAsync(It.IsAny<PalierFamilial>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PalierFamilial e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_Accepte_Reduction100()
    {
        var entity = CreatePalier();
        entity.ReductionPourcent = 100m;

        _repoMock.Setup(r => r.AddAsync(It.IsAny<PalierFamilial>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PalierFamilial e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_RetourneTrue_QuandSuppressionReussie()
    {
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.DeleteAsync(1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_RetourneFalse_QuandInexistant()
    {
        _repoMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.DeleteAsync(999);

        result.Should().BeFalse();
    }

    // --- Helper ---

    private static PalierFamilial CreatePalier()
    {
        return new PalierFamilial
        {
            Id = 1,
            CompanyId = 1,
            RangEnfant = 2,
            ReductionPourcent = 10m
        };
    }
}
