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
/// Tests unitaires pour DepenseService.
/// </summary>
public sealed class DepenseServiceTests
{
    private readonly Mock<IDepenseRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<DepenseService>> _loggerMock;
    private readonly DepenseService _sut;

    public DepenseServiceTests()
    {
        _repoMock = new Mock<IDepenseRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<DepenseService>>();

        _sut = new DepenseService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_RetourneDepense_QuandExiste()
    {
        var entity = CreateDepense();
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
            .ReturnsAsync((Depense?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // --- GetPagedAsync ---

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Depense>(
            new List<Depense> { CreateDepense() }, 1, 1, 20);
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_CreeLaDepense_AvecDonneesValides()
    {
        var entity = CreateDepense();
        _repoMock.Setup(r => r.GetByNumeroJustificatifAsync(entity.NumeroJustificatif, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Depense?)null);
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
        var entity = CreateDepense();
        _repoMock.Setup(r => r.GetByNumeroJustificatifAsync(entity.NumeroJustificatif, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Depense?)null);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Depense>>(),
            "kouroukan.events",
            "entity.created.depense",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiNumeroJustificatifDuplique()
    {
        var entity = CreateDepense();
        _repoMock.Setup(r => r.GetByNumeroJustificatifAsync(entity.NumeroJustificatif, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*numero de justificatif*existe deja*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMontantZeroOuNegatif()
    {
        var entity = CreateDepense(montant: 0m);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant*superieur a zero*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCategorieInvalide()
    {
        var entity = CreateDepense(categorie: "INVALIDE");
        _repoMock.Setup(r => r.GetByNumeroJustificatifAsync(entity.NumeroJustificatif, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Depense?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*categorie*n'est pas autorisee*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateDepense(statut: "INVALIDE");
        _repoMock.Setup(r => r.GetByNumeroJustificatifAsync(entity.NumeroJustificatif, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Depense?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*statut*n'est pas autorise*");
    }

    [Theory]
    [InlineData("Personnel")]
    [InlineData("Fournitures")]
    [InlineData("Maintenance")]
    [InlineData("Evenements")]
    [InlineData("BDE")]
    [InlineData("Equipements")]
    public async Task CreateAsync_AccepteToutesLesCategoriesAutorisees(string categorie)
    {
        var entity = CreateDepense(categorie: categorie);
        _repoMock.Setup(r => r.GetByNumeroJustificatifAsync(entity.NumeroJustificatif, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Depense?)null);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("Demande")]
    [InlineData("ValideN1")]
    [InlineData("ValideFinance")]
    [InlineData("ValideDirection")]
    [InlineData("Executee")]
    [InlineData("Archivee")]
    public async Task CreateAsync_AccepteTousLesStatutsAutorises(string statut)
    {
        var entity = CreateDepense(statut: statut);
        _repoMock.Setup(r => r.GetByNumeroJustificatifAsync(entity.NumeroJustificatif, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Depense?)null);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateDepense();
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateDepense();
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Depense>>(),
            "kouroukan.events",
            "entity.updated.depense",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateDepense();
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Depense>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiMontantZero()
    {
        var entity = CreateDepense(montant: 0m);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant*superieur a zero*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiCategorieInvalide()
    {
        var entity = CreateDepense(categorie: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*categorie*n'est pas autorisee*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateDepense(statut: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*statut*n'est pas autorise*");
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
            It.IsAny<EntityDeletedEvent<Depense>>(),
            "kouroukan.events",
            "entity.deleted.depense",
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

    private static Depense CreateDepense(
        decimal montant = 500000m,
        string categorie = "Fournitures",
        string statut = "Demande")
    {
        return new Depense
        {
            Id = 1,
            TypeId = 1,
            Montant = montant,
            MotifDepense = "Achat fournitures scolaires",
            Categorie = categorie,
            BeneficiaireNom = "Fournisseur ABC",
            BeneficiaireTelephone = "622000000",
            StatutDepense = statut,
            DateDemande = new DateTime(2025, 9, 1),
            NumeroJustificatif = "DEP-2025-001",
            UserId = 1
        };
    }
}
