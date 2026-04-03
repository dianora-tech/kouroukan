using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Output;
using Bde.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Bde.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour DepenseBdeService.
/// </summary>
public sealed class DepenseBdeServiceTests
{
    private readonly Mock<IDepenseBdeRepository> _repoMock;
    private readonly Mock<IAssociationRepository> _associationRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<DepenseBdeService>> _loggerMock;
    private readonly DepenseBdeService _sut;

    public DepenseBdeServiceTests()
    {
        _repoMock = new Mock<IDepenseBdeRepository>();
        _associationRepoMock = new Mock<IAssociationRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<DepenseBdeService>>();

        _sut = new DepenseBdeService(
            _repoMock.Object,
            _associationRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneDepense_QuandExiste()
    {
        var depense = CreateDepense();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(depense);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DepenseBde?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var depenses = new List<DepenseBde>
        {
            CreateDepense(),
            CreateDepense(id: 2, name: "Location salle")
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(depenses);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<DepenseBde>(
            new List<DepenseBde> { CreateDepense() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLaDepense_AvecDonneesValides()
    {
        var entity = CreateDepense();

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
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

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<DepenseBde>>(),
            "kouroukan.events",
            "entity.created.depensebde",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiStatutValidationInvalide()
    {
        var entity = CreateDepense(statutValidation: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut de validation invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCategorieInvalide()
    {
        var entity = CreateDepense(categorie: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Categorie invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMontantZero()
    {
        var entity = CreateDepense();
        entity.Montant = 0m;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant*superieur a zero*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMontantNegatif()
    {
        var entity = CreateDepense();
        entity.Montant = -100m;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant*superieur a zero*");
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiAssociationInexistante()
    {
        var entity = CreateDepense();

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*association*n'existe pas*");
    }

    [Theory]
    [InlineData("Demandee")]
    [InlineData("ValideTresorier")]
    [InlineData("ValideSuper")]
    [InlineData("Refusee")]
    public async Task CreateAsync_AccepteTousLesStatutsValides(string statut)
    {
        var entity = CreateDepense(statutValidation: statut);

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("Materiel")]
    [InlineData("Location")]
    [InlineData("Prestataire")]
    [InlineData("Remboursement")]
    public async Task CreateAsync_AccepteToutesLesCategoriesValides(string categorie)
    {
        var entity = CreateDepense(categorie: categorie);

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // ─── UpdateAsync ───

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
            It.IsAny<EntityUpdatedEvent<DepenseBde>>(),
            "kouroukan.events",
            "entity.updated.depensebde",
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
            It.IsAny<EntityUpdatedEvent<DepenseBde>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiStatutValidationInvalide()
    {
        var entity = CreateDepense(statutValidation: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut de validation invalide*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiCategorieInvalide()
    {
        var entity = CreateDepense(categorie: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Categorie invalide*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiMontantNegatif()
    {
        var entity = CreateDepense();
        entity.Montant = -50m;

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant*superieur a zero*");
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
            It.IsAny<EntityDeletedEvent<DepenseBde>>(),
            "kouroukan.events",
            "entity.deleted.depensebde",
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
            It.IsAny<EntityDeletedEvent<DepenseBde>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static DepenseBde CreateDepense(
        int id = 1,
        string name = "Achat materiel",
        string statutValidation = "Demandee",
        string categorie = "Materiel")
    {
        return new DepenseBde
        {
            Id = id,
            TypeId = 1,
            Name = name,
            Description = "Achat de fournitures",
            AssociationId = 5,
            Montant = 100000m,
            Motif = "Evenement de rentree",
            Categorie = categorie,
            StatutValidation = statutValidation,
            ValidateurId = 10,
            UserId = 1
        };
    }
}
