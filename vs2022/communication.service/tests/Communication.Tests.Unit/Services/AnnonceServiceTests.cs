using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Output;
using Communication.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Communication.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour AnnonceService.
/// </summary>
public sealed class AnnonceServiceTests
{
    private readonly Mock<IAnnonceRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<AnnonceService>> _loggerMock;
    private readonly AnnonceService _sut;

    public AnnonceServiceTests()
    {
        _repoMock = new Mock<IAnnonceRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<AnnonceService>>();

        _sut = new AnnonceService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneAnnonce_QuandExiste()
    {
        var annonce = CreateAnnonce();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(annonce);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Annonce?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var annonces = new List<Annonce>
        {
            CreateAnnonce(),
            CreateAnnonce(id: 2)
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(annonces);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Annonce>(
            new List<Annonce> { CreateAnnonce() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLAnnonce_AvecDonneesValides()
    {
        var entity = CreateAnnonce();

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
        var entity = CreateAnnonce();

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Annonce>>(),
            "kouroukan.events",
            "entity.created.annonce",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiContenuVide()
    {
        var entity = CreateAnnonce();
        entity.Contenu = "";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*contenu de l'annonce est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiContenuNull()
    {
        var entity = CreateAnnonce();
        entity.Contenu = "   ";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*contenu de l'annonce est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiDateFinAvantDateDebut()
    {
        var entity = CreateAnnonce();
        entity.DateDebut = new DateTime(2025, 9, 1);
        entity.DateFin = new DateTime(2025, 8, 1);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*date de fin doit etre posterieure*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiPrioriteInferieure1()
    {
        var entity = CreateAnnonce();
        entity.Priorite = 0;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*priorite doit etre superieure ou egale a 1*");
    }

    [Fact]
    public async Task CreateAsync_Accepte_DateFinNull()
    {
        var entity = CreateAnnonce();
        entity.DateFin = null;

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_Accepte_DateFinApreeDateDebut()
    {
        var entity = CreateAnnonce();
        entity.DateDebut = new DateTime(2025, 9, 1);
        entity.DateFin = new DateTime(2025, 12, 31);

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateAnnonce();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateAnnonce();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Annonce>>(),
            "kouroukan.events",
            "entity.updated.annonce",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateAnnonce();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Annonce>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_RetourneFalse_QuandInexistante()
    {
        var entity = CreateAnnonce();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiDateFinAvantDateDebut()
    {
        var entity = CreateAnnonce();
        entity.DateDebut = new DateTime(2025, 9, 1);
        entity.DateFin = new DateTime(2025, 8, 1);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*date de fin doit etre posterieure*");
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
            It.IsAny<EntityDeletedEvent<Annonce>>(),
            "kouroukan.events",
            "entity.deleted.annonce",
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
            It.IsAny<EntityDeletedEvent<Annonce>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static Annonce CreateAnnonce(int id = 1)
    {
        return new Annonce
        {
            Id = id,
            Name = "Annonce de test",
            TypeId = 1,
            Contenu = "Contenu de l'annonce de test",
            DateDebut = new DateTime(2025, 9, 1),
            DateFin = new DateTime(2025, 12, 31),
            EstActive = true,
            CibleAudience = "Tous",
            Priorite = 1,
            UserId = 1
        };
    }
}
