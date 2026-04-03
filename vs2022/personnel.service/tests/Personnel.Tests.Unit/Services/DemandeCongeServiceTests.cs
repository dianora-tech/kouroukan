using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Output;
using Personnel.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Personnel.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour DemandeCongeService.
/// </summary>
public sealed class DemandeCongeServiceTests
{
    private readonly Mock<IDemandeCongeRepository> _repoMock;
    private readonly Mock<IEnseignantRepository> _enseignantRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<DemandeCongeService>> _loggerMock;
    private readonly DemandeCongeService _sut;

    public DemandeCongeServiceTests()
    {
        _repoMock = new Mock<IDemandeCongeRepository>();
        _enseignantRepoMock = new Mock<IEnseignantRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<DemandeCongeService>>();

        _sut = new DemandeCongeService(
            _repoMock.Object,
            _enseignantRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneDemandeConge_QuandExiste()
    {
        var demande = CreateDemandeConge();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DemandeConge?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var demandes = new List<DemandeConge>
        {
            CreateDemandeConge(id: 1),
            CreateDemandeConge(id: 2)
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(demandes);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<DemandeConge>(
            new List<DemandeConge> { CreateDemandeConge() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeDemandeConge_AvecDatesValides()
    {
        var entity = CreateDemandeConge();

        _enseignantRepoMock.Setup(r => r.GetByIdAsync(entity.EnseignantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Enseignant { Id = entity.EnseignantId });
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
        var entity = CreateDemandeConge();

        _enseignantRepoMock.Setup(r => r.GetByIdAsync(entity.EnseignantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Enseignant { Id = entity.EnseignantId });
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<DemandeConge>>(),
            "kouroukan.events",
            "entity.created.demandeconge",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiEnseignantInexistant()
    {
        var entity = CreateDemandeConge();

        _enseignantRepoMock.Setup(r => r.GetByIdAsync(entity.EnseignantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enseignant?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*enseignant*n'existe pas*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiDateFinAnterieureDateDebut()
    {
        var entity = CreateDemandeConge();
        entity.DateFin = entity.DateDebut.AddDays(-1);

        _enseignantRepoMock.Setup(r => r.GetByIdAsync(entity.EnseignantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Enseignant { Id = entity.EnseignantId });

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*date de fin*posterieure*date de debut*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiDateFinEgaleDateDebut()
    {
        var entity = CreateDemandeConge();
        entity.DateFin = entity.DateDebut;

        _enseignantRepoMock.Setup(r => r.GetByIdAsync(entity.EnseignantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Enseignant { Id = entity.EnseignantId });

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*date de fin*posterieure*date de debut*");
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateDemandeConge();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateDemandeConge();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<DemandeConge>>(),
            "kouroukan.events",
            "entity.updated.demandeconge",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateDemandeConge();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<DemandeConge>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_RetourneFalse_QuandInexistante()
    {
        var entity = CreateDemandeConge();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiDateFinAnterieureDateDebut()
    {
        var entity = CreateDemandeConge();
        entity.DateFin = entity.DateDebut.AddDays(-1);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*date de fin*posterieure*date de debut*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiDateFinEgaleDateDebut()
    {
        var entity = CreateDemandeConge();
        entity.DateFin = entity.DateDebut;

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*date de fin*posterieure*date de debut*");
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
            It.IsAny<EntityDeletedEvent<DemandeConge>>(),
            "kouroukan.events",
            "entity.deleted.demandeconge",
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
            It.IsAny<EntityDeletedEvent<DemandeConge>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static DemandeConge CreateDemandeConge(int id = 1)
    {
        return new DemandeConge
        {
            Id = id,
            Name = "Conge annuel",
            Description = "Conge annuel de fin d'annee",
            EnseignantId = 10,
            DateDebut = new DateTime(2025, 7, 1),
            DateFin = new DateTime(2025, 7, 15),
            Motif = "Repos annuel",
            StatutDemande = "Soumise",
            PieceJointeUrl = null,
            CommentaireValidateur = null,
            ValidateurId = null,
            DateValidation = null,
            ImpactPaie = false,
            TypeId = 1,
            UserId = 1
        };
    }
}
