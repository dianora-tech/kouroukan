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
/// Tests unitaires pour EvenementService.
/// </summary>
public sealed class EvenementServiceTests
{
    private readonly Mock<IEvenementRepository> _repoMock;
    private readonly Mock<IAssociationRepository> _associationRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<EvenementService>> _loggerMock;
    private readonly EvenementService _sut;

    public EvenementServiceTests()
    {
        _repoMock = new Mock<IEvenementRepository>();
        _associationRepoMock = new Mock<IAssociationRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<EvenementService>>();

        _sut = new EvenementService(
            _repoMock.Object,
            _associationRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneEvenement_QuandExiste()
    {
        var evenement = CreateEvenement();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evenement);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Evenement?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var evenements = new List<Evenement>
        {
            CreateEvenement(),
            CreateEvenement(id: 2, name: "Soiree de gala")
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(evenements);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Evenement>(
            new List<Evenement> { CreateEvenement() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLEvenement_AvecDonneesValides()
    {
        var entity = CreateEvenement();

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
        var entity = CreateEvenement();

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Evenement>>(),
            "kouroukan.events",
            "entity.created.evenement",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateEvenement(statutEvenement: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut d'evenement invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiAssociationInexistante()
    {
        var entity = CreateEvenement();

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*association*n'existe pas*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiNombreInscritsDepasseCapacite()
    {
        var entity = CreateEvenement();
        entity.Capacite = 50;
        entity.NombreInscrits = 51;

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*nombre d'inscrits depasse*capacite*");
    }

    [Fact]
    public async Task CreateAsync_AccepteInscritsEgalCapacite()
    {
        var entity = CreateEvenement();
        entity.Capacite = 50;
        entity.NombreInscrits = 50;

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_AccepteSansCapacite()
    {
        var entity = CreateEvenement();
        entity.Capacite = null;
        entity.NombreInscrits = 1000;

        _associationRepoMock.Setup(r => r.ExistsAsync(entity.AssociationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("Planifie")]
    [InlineData("Valide")]
    [InlineData("EnCours")]
    [InlineData("Termine")]
    [InlineData("Annule")]
    public async Task CreateAsync_AccepteTousLesStatutsValides(string statut)
    {
        var entity = CreateEvenement(statutEvenement: statut);

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
        var entity = CreateEvenement();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateEvenement();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Evenement>>(),
            "kouroukan.events",
            "entity.updated.evenement",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateEvenement();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Evenement>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateEvenement(statutEvenement: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut d'evenement invalide*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiNombreInscritsDepasseCapacite()
    {
        var entity = CreateEvenement();
        entity.Capacite = 10;
        entity.NombreInscrits = 11;

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*nombre d'inscrits depasse*capacite*");
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
            It.IsAny<EntityDeletedEvent<Evenement>>(),
            "kouroukan.events",
            "entity.deleted.evenement",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_RetourneFalse_QuandInexistant()
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
            It.IsAny<EntityDeletedEvent<Evenement>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static Evenement CreateEvenement(
        int id = 1,
        string name = "Journee integration",
        string statutEvenement = "Planifie")
    {
        return new Evenement
        {
            Id = id,
            TypeId = 1,
            Name = name,
            Description = "Evenement de bienvenue",
            AssociationId = 5,
            DateEvenement = new DateTime(2025, 10, 15),
            Lieu = "Campus principal",
            Capacite = 200,
            TarifEntree = 5000m,
            NombreInscrits = 50,
            StatutEvenement = statutEvenement,
            UserId = 1
        };
    }
}
