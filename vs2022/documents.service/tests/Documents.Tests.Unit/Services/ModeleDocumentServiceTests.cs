using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Output;
using Documents.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Documents.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour ModeleDocumentService.
/// </summary>
public sealed class ModeleDocumentServiceTests
{
    private readonly Mock<IModeleDocumentRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<ModeleDocumentService>> _loggerMock;
    private readonly ModeleDocumentService _sut;

    public ModeleDocumentServiceTests()
    {
        _repoMock = new Mock<IModeleDocumentRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<ModeleDocumentService>>();

        _sut = new ModeleDocumentService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneModeleDocument_QuandExiste()
    {
        var entity = CreateModeleDocument();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ModeleDocument?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var entities = new List<ModeleDocument> { CreateModeleDocument(), CreateModeleDocument(id: 2) };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<ModeleDocument>(
            new List<ModeleDocument> { CreateModeleDocument() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeModeleDocument_AvecDonneesValides()
    {
        var entity = CreateModeleDocument();

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
        var entity = CreateModeleDocument();

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<ModeleDocument>>(),
            "kouroukan.events",
            "entity.created.modeledocument",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCodeVide()
    {
        var entity = CreateModeleDocument();
        entity.Code = "";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*code du modele*obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCodeWhitespace()
    {
        var entity = CreateModeleDocument();
        entity.Code = "   ";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*code du modele*obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiContenuVide()
    {
        var entity = CreateModeleDocument();
        entity.Contenu = "";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*contenu du modele*obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiContenuWhitespace()
    {
        var entity = CreateModeleDocument();
        entity.Contenu = "   ";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*contenu du modele*obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCouleurPrimaireSansHash()
    {
        var entity = CreateModeleDocument();
        entity.CouleurPrimaire = "16a34a";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*couleur primaire*hexadecimal*");
    }

    [Fact]
    public async Task CreateAsync_AccepteCouleurPrimaireAvecHash()
    {
        var entity = CreateModeleDocument();
        entity.CouleurPrimaire = "#16a34a";

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_AccepteCouleurPrimaireNull()
    {
        var entity = CreateModeleDocument();
        entity.CouleurPrimaire = null;

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateModeleDocument();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateModeleDocument();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<ModeleDocument>>(),
            "kouroukan.events",
            "entity.updated.modeledocument",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateModeleDocument();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<ModeleDocument>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiCodeVide()
    {
        var entity = CreateModeleDocument();
        entity.Code = "";

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*code du modele*obligatoire*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiContenuVide()
    {
        var entity = CreateModeleDocument();
        entity.Contenu = "";

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*contenu du modele*obligatoire*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiCouleurPrimaireSansHash()
    {
        var entity = CreateModeleDocument();
        entity.CouleurPrimaire = "ff0000";

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*couleur primaire*hexadecimal*");
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
            It.IsAny<EntityDeletedEvent<ModeleDocument>>(),
            "kouroukan.events",
            "entity.deleted.modeledocument",
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
            It.IsAny<EntityDeletedEvent<ModeleDocument>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static ModeleDocument CreateModeleDocument(int id = 1)
    {
        return new ModeleDocument
        {
            Id = id,
            TypeId = 1,
            Code = "BULL_NOTES",
            Contenu = "<html><body>{{nom}}</body></html>",
            LogoUrl = "https://example.com/logo.png",
            CouleurPrimaire = "#16a34a",
            TextePiedPage = "Pied de page",
            EstActif = true,
            UserId = 1
        };
    }
}
