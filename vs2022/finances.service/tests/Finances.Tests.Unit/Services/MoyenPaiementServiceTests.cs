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
/// Tests unitaires pour MoyenPaiementService.
/// </summary>
public sealed class MoyenPaiementServiceTests
{
    private readonly Mock<IMoyenPaiementRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<MoyenPaiementService>> _loggerMock;
    private readonly MoyenPaiementService _sut;

    public MoyenPaiementServiceTests()
    {
        _repoMock = new Mock<IMoyenPaiementRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<MoyenPaiementService>>();

        _sut = new MoyenPaiementService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneMoyenPaiement_QuandExiste()
    {
        var entity = CreateMoyenPaiement();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_CreeLeMoyenPaiement_AvecDonneesValides()
    {
        var entity = CreateMoyenPaiement();
        _repoMock.Setup(r => r.AddAsync(It.IsAny<MoyenPaiement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MoyenPaiement e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        result.EstActif.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreateMoyenPaiement();
        _repoMock.Setup(r => r.AddAsync(It.IsAny<MoyenPaiement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MoyenPaiement e, CancellationToken _) => e);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<MoyenPaiement>>(),
            "kouroukan.events",
            "entity.created.moyen-paiement",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiOperateurVide()
    {
        var entity = CreateMoyenPaiement();
        entity.Operateur = "";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*operateur est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_ForceEstActifATrue()
    {
        var entity = CreateMoyenPaiement();
        entity.EstActif = false;

        _repoMock.Setup(r => r.AddAsync(It.IsAny<MoyenPaiement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MoyenPaiement e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.EstActif.Should().BeTrue();
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandReussi()
    {
        var entity = CreateMoyenPaiement();
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateMoyenPaiement();
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<MoyenPaiement>>(),
            "kouroukan.events",
            "entity.updated.moyen-paiement",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
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

    private static MoyenPaiement CreateMoyenPaiement()
    {
        return new MoyenPaiement
        {
            Id = 1,
            CompanyId = 1,
            Operateur = "OrangeMoney",
            NumeroCompte = "622000000",
            CodeMarchand = "MARC-001",
            Libelle = "Orange Money Ecole",
            EstActif = true
        };
    }
}
