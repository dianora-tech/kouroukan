using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Output;
using Evaluations.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Evaluations.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour BulletinService.
/// </summary>
public sealed class BulletinServiceTests
{
    private readonly Mock<IBulletinRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<BulletinService>> _loggerMock;
    private readonly BulletinService _sut;

    public BulletinServiceTests()
    {
        _repoMock = new Mock<IBulletinRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<BulletinService>>();

        _sut = new BulletinService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneBulletin_QuandExiste()
    {
        var bulletin = CreateBulletin();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bulletin);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bulletin?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var bulletins = new List<Bulletin>
        {
            CreateBulletin(),
            CreateBulletin(id: 2)
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bulletins);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Bulletin>(
            new List<Bulletin> { CreateBulletin() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLeBulletin_AvecDonneesValides()
    {
        var entity = CreateBulletin();

        _repoMock.Setup(r => r.GetByEleveTrimestreAsync(
                entity.EleveId, entity.Trimestre, entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bulletin?)null);
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
        var entity = CreateBulletin();

        _repoMock.Setup(r => r.GetByEleveTrimestreAsync(
                entity.EleveId, entity.Trimestre, entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bulletin?)null);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Bulletin>>(),
            "kouroukan.events",
            "entity.created.bulletin",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(-1)]
    public async Task CreateAsync_Lance_InvalidOperationException_SiTrimestreInvalide(int trimestre)
    {
        var entity = CreateBulletin(trimestre: trimestre);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*trimestre*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(21)]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMoyenneGeneraleInvalide(decimal moyenne)
    {
        var entity = CreateBulletin(moyenneGenerale: moyenne);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*moyenne generale*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiBulletinExisteDeja()
    {
        var entity = CreateBulletin();
        var existing = CreateBulletin(id: 99);

        _repoMock.Setup(r => r.GetByEleveTrimestreAsync(
                entity.EleveId, entity.Trimestre, entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*bulletin existe deja*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CreateAsync_AccepteTousLesTrimestresValides(int trimestre)
    {
        var entity = CreateBulletin(trimestre: trimestre);

        _repoMock.Setup(r => r.GetByEleveTrimestreAsync(
                entity.EleveId, entity.Trimestre, entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bulletin?)null);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(20)]
    public async Task CreateAsync_AccepteMoyennesValides(decimal moyenne)
    {
        var entity = CreateBulletin(moyenneGenerale: moyenne);

        _repoMock.Setup(r => r.GetByEleveTrimestreAsync(
                entity.EleveId, entity.Trimestre, entity.AnneeScolaireId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bulletin?)null);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateBulletin();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateBulletin();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Bulletin>>(),
            "kouroukan.events",
            "entity.updated.bulletin",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateBulletin();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Bulletin>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiTrimestreInvalide(int trimestre)
    {
        var entity = CreateBulletin(trimestre: trimestre);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*trimestre*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(21)]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiMoyenneGeneraleInvalide(decimal moyenne)
    {
        var entity = CreateBulletin(moyenneGenerale: moyenne);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*moyenne generale*");
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
            It.IsAny<EntityDeletedEvent<Bulletin>>(),
            "kouroukan.events",
            "entity.deleted.bulletin",
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
            It.IsAny<EntityDeletedEvent<Bulletin>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static Bulletin CreateBulletin(
        int id = 1,
        int trimestre = 1,
        decimal moyenneGenerale = 14.5m)
    {
        return new Bulletin
        {
            Id = id,
            EleveId = 5,
            ClasseId = 3,
            Trimestre = trimestre,
            AnneeScolaireId = 1,
            MoyenneGenerale = moyenneGenerale,
            Rang = 3,
            Appreciation = "Bon trimestre",
            EstPublie = false,
            DateGeneration = new DateTime(2025, 12, 20),
            CheminFichierPdf = null,
            UserId = 1
        };
    }
}
