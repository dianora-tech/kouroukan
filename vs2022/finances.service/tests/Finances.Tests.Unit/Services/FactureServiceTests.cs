using Finances.Domain.Entities;
using Finances.Domain.Ports.Output;
using Finances.Domain.Services;
using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour FactureService.
/// </summary>
public sealed class FactureServiceTests
{
    private readonly Mock<IFactureRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<FactureService>> _loggerMock;
    private readonly FactureService _sut;

    public FactureServiceTests()
    {
        _repoMock = new Mock<IFactureRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<FactureService>>();

        _sut = new FactureService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_RetourneFacture_QuandExiste()
    {
        var entity = CreateFacture();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facture?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // --- GetPagedAsync ---

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Facture>(
            new List<Facture> { CreateFacture() }, 1, 1, 20);
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_CreeLaFacture_AvecDonneesValides()
    {
        var entity = CreateFacture();
        _repoMock.Setup(r => r.GetByNumeroFactureAsync(entity.NumeroFacture, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facture?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CalculeLeSolde()
    {
        var entity = CreateFacture();
        entity.MontantTotal = 200000m;
        entity.MontantPaye = 50000m;

        _repoMock.Setup(r => r.GetByNumeroFactureAsync(entity.NumeroFacture, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facture?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facture e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.Solde.Should().Be(150000m);
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreateFacture();
        _repoMock.Setup(r => r.GetByNumeroFactureAsync(entity.NumeroFacture, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facture?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Facture>>(),
            "kouroukan.events",
            "entity.created.facture",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiNumeroFactureDuplique()
    {
        var entity = CreateFacture();
        _repoMock.Setup(r => r.GetByNumeroFactureAsync(entity.NumeroFacture, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*numero*existe deja*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMontantTotalNegatif()
    {
        var entity = CreateFacture();
        entity.MontantTotal = -100m;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant total ne peut pas etre negatif*");
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateFacture();
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_CalculeLeSolde()
    {
        var entity = CreateFacture();
        entity.MontantTotal = 300000m;
        entity.MontantPaye = 100000m;

        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        entity.Solde.Should().Be(200000m);
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateFacture();
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Facture>>(),
            "kouroukan.events",
            "entity.updated.facture",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateFacture();
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Facture>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiMontantTotalNegatif()
    {
        var entity = CreateFacture();
        entity.MontantTotal = -1m;

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant total ne peut pas etre negatif*");
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
    public async Task DeleteAsync_PublieEvenement_SiReussite()
    {
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.DeleteAsync(1);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityDeletedEvent<Facture>>(),
            "kouroukan.events",
            "entity.deleted.facture",
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

    // --- Helper ---

    private static Facture CreateFacture()
    {
        return new Facture
        {
            Id = 1,
            TypeId = 1,
            EleveId = 10,
            ParentId = 5,
            AnneeScolaireId = 1,
            MontantTotal = 150000m,
            MontantPaye = 0m,
            Solde = 150000m,
            DateEmission = new DateTime(2025, 9, 1),
            DateEcheance = new DateTime(2025, 12, 31),
            StatutFacture = "Emise",
            NumeroFacture = "FAC-2025-001",
            UserId = 1
        };
    }
}
