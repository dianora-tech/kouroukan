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
/// Tests unitaires pour InscriptionService.
/// </summary>
public sealed class InscriptionServiceTests
{
    private readonly Mock<IInscriptionRepository> _repoMock;
    private readonly Mock<IEleveRepository> _eleveRepoMock;
    private readonly Mock<IAnneeScolaireRepository> _anneeScolaireRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<InscriptionService>> _loggerMock;
    private readonly InscriptionService _sut;

    public InscriptionServiceTests()
    {
        _repoMock = new Mock<IInscriptionRepository>();
        _eleveRepoMock = new Mock<IEleveRepository>();
        _anneeScolaireRepoMock = new Mock<IAnneeScolaireRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<InscriptionService>>();

        _sut = new InscriptionService(
            _repoMock.Object,
            _eleveRepoMock.Object,
            _anneeScolaireRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneInscription_QuandExiste()
    {
        var inscription = CreateInscription();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inscription);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Inscription?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Inscription>(
            new List<Inscription> { CreateInscription() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLInscription_AvecStatutValide()
    {
        var entity = CreateInscription(statutInscription: "EnAttente");

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
    public async Task CreateAsync_PublieEvenement_ApreCreation()
    {
        var entity = CreateInscription();

        _eleveRepoMock.Setup(r => r.ExistsAsync(entity.EleveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _anneeScolaireRepoMock.Setup(r => r.ExistsAsync(entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Inscription>>(),
            "kouroukan.events",
            "entity.created.inscription",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateInscription(statutInscription: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut d'inscription invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiTypeEtablissementInvalide()
    {
        var entity = CreateInscription();
        entity.TypeEtablissement = "Inconnu";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Type d'etablissement invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiSerieBacInvalide()
    {
        var entity = CreateInscription();
        entity.SerieBac = "XX";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Serie du bac invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiEleveInexistant()
    {
        var entity = CreateInscription();

        _eleveRepoMock.Setup(r => r.ExistsAsync(entity.EleveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*L'eleve*n'existe pas*");
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiAnneeScolaireInexistante()
    {
        var entity = CreateInscription();

        _eleveRepoMock.Setup(r => r.ExistsAsync(entity.EleveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _anneeScolaireRepoMock.Setup(r => r.ExistsAsync(entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*annee scolaire*n'existe pas*");
    }

    [Theory]
    [InlineData("EnAttente")]
    [InlineData("Validee")]
    [InlineData("Annulee")]
    public async Task CreateAsync_AccepteTousLesStatutsValides(string statut)
    {
        var entity = CreateInscription(statutInscription: statut);

        _eleveRepoMock.Setup(r => r.ExistsAsync(entity.EleveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _anneeScolaireRepoMock.Setup(r => r.ExistsAsync(entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
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
        var entity = CreateInscription();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateInscription();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Inscription>>(),
            "kouroukan.events",
            "entity.updated.inscription",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateInscription();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Inscription>>(),
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
            It.IsAny<EntityDeletedEvent<Inscription>>(),
            "kouroukan.events",
            "entity.deleted.inscription",
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

    private static Inscription CreateInscription(
        string statutInscription = "EnAttente",
        string? typeEtablissement = null,
        string? serieBac = null)
    {
        return new Inscription
        {
            Id = 1,
            TypeId = 1,
            EleveId = 10,
            ClasseId = 5,
            AnneeScolaireId = 1,
            DateInscription = new DateTime(2025, 9, 1),
            MontantInscription = 150000m,
            EstPaye = false,
            EstRedoublant = false,
            TypeEtablissement = typeEtablissement,
            SerieBac = serieBac,
            StatutInscription = statutInscription,
            UserId = 1
        };
    }
}
