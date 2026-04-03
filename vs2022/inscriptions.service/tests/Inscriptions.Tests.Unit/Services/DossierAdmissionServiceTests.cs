using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Output;
using Inscriptions.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour DossierAdmissionService.
/// </summary>
public sealed class DossierAdmissionServiceTests
{
    private readonly Mock<IDossierAdmissionRepository> _repoMock;
    private readonly Mock<IEleveRepository> _eleveRepoMock;
    private readonly Mock<IAnneeScolaireRepository> _anneeScolaireRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<DossierAdmissionService>> _loggerMock;
    private readonly DossierAdmissionService _sut;

    public DossierAdmissionServiceTests()
    {
        _repoMock = new Mock<IDossierAdmissionRepository>();
        _eleveRepoMock = new Mock<IEleveRepository>();
        _anneeScolaireRepoMock = new Mock<IAnneeScolaireRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<DossierAdmissionService>>();

        _sut = new DossierAdmissionService(
            _repoMock.Object,
            _eleveRepoMock.Object,
            _anneeScolaireRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneDossier_QuandExiste()
    {
        var dossier = CreateDossier();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dossier);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DossierAdmission?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<DossierAdmission>(
            new List<DossierAdmission> { CreateDossier() }, 1, 1, 20);
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLeDossier_AvecStatutValide()
    {
        var entity = CreateDossier();

        _eleveRepoMock.Setup(r => r.ExistsAsync(entity.EleveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _anneeScolaireRepoMock.Setup(r => r.ExistsAsync(entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
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
        var entity = CreateDossier();

        _eleveRepoMock.Setup(r => r.ExistsAsync(entity.EleveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _anneeScolaireRepoMock.Setup(r => r.ExistsAsync(entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<DossierAdmission>>(),
            "kouroukan.events",
            "entity.created.dossieradmission",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateDossier(statutDossier: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut de dossier invalide*");
    }

    [Theory]
    [InlineData("Prospect")]
    [InlineData("PreInscrit")]
    [InlineData("EnEtude")]
    [InlineData("Convoque")]
    [InlineData("Admis")]
    [InlineData("Refuse")]
    [InlineData("ListeAttente")]
    public async Task CreateAsync_AccepteTousLesStatutsValides(string statut)
    {
        var entity = CreateDossier(statutDossier: statut);

        _eleveRepoMock.Setup(r => r.ExistsAsync(entity.EleveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _anneeScolaireRepoMock.Setup(r => r.ExistsAsync(entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiEleveInexistant()
    {
        var entity = CreateDossier();

        _eleveRepoMock.Setup(r => r.ExistsAsync(entity.EleveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*eleve*n'existe pas*");
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiAnneeScolaireInexistante()
    {
        var entity = CreateDossier();

        _eleveRepoMock.Setup(r => r.ExistsAsync(entity.EleveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _anneeScolaireRepoMock.Setup(r => r.ExistsAsync(entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*annee scolaire*n'existe pas*");
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateDossier();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateDossier();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<DossierAdmission>>(),
            "kouroukan.events",
            "entity.updated.dossieradmission",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateDossier();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<DossierAdmission>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateDossier(statutDossier: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut de dossier invalide*");
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
            It.IsAny<EntityDeletedEvent<DossierAdmission>>(),
            "kouroukan.events",
            "entity.deleted.dossieradmission",
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

    // ─── Helper ───

    private static DossierAdmission CreateDossier(
        string statutDossier = "EnEtude")
    {
        return new DossierAdmission
        {
            Id = 1,
            TypeId = 1,
            EleveId = 10,
            AnneeScolaireId = 1,
            StatutDossier = statutDossier,
            EtapeActuelle = "DepotDossier",
            DateDemande = new DateTime(2025, 6, 1),
            UserId = 1
        };
    }
}
