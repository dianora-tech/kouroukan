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
/// Tests unitaires pour DocumentGenereService.
/// </summary>
public sealed class DocumentGenereServiceTests
{
    private readonly Mock<IDocumentGenereRepository> _repoMock;
    private readonly Mock<IModeleDocumentRepository> _modeleDocRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<DocumentGenereService>> _loggerMock;
    private readonly DocumentGenereService _sut;

    public DocumentGenereServiceTests()
    {
        _repoMock = new Mock<IDocumentGenereRepository>();
        _modeleDocRepoMock = new Mock<IModeleDocumentRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<DocumentGenereService>>();

        _sut = new DocumentGenereService(
            _repoMock.Object,
            _modeleDocRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneDocumentGenere_QuandExiste()
    {
        var entity = CreateDocumentGenere();
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
            .ReturnsAsync((DocumentGenere?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var entities = new List<DocumentGenere> { CreateDocumentGenere(), CreateDocumentGenere(id: 2) };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<DocumentGenere>(
            new List<DocumentGenere> { CreateDocumentGenere() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeDocumentGenere_AvecStatutValide()
    {
        var entity = CreateDocumentGenere(statutSignature: "EnAttente");

        _modeleDocRepoMock.Setup(r => r.ExistsAsync(entity.ModeleDocumentId, It.IsAny<CancellationToken>()))
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
        var entity = CreateDocumentGenere();

        _modeleDocRepoMock.Setup(r => r.ExistsAsync(entity.ModeleDocumentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<DocumentGenere>>(),
            "kouroukan.events",
            "entity.created.documentgenere",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiStatutSignatureInvalide()
    {
        var entity = CreateDocumentGenere(statutSignature: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut de signature invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiModeleDocumentInexistant()
    {
        var entity = CreateDocumentGenere();

        _modeleDocRepoMock.Setup(r => r.ExistsAsync(entity.ModeleDocumentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*modele de document*n'existe pas*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiDonneesJsonVide()
    {
        var entity = CreateDocumentGenere();
        entity.DonneesJson = "";

        _modeleDocRepoMock.Setup(r => r.ExistsAsync(entity.ModeleDocumentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*donnees JSON*obligatoires*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiDonneesJsonWhitespace()
    {
        var entity = CreateDocumentGenere();
        entity.DonneesJson = "   ";

        _modeleDocRepoMock.Setup(r => r.ExistsAsync(entity.ModeleDocumentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*donnees JSON*obligatoires*");
    }

    [Theory]
    [InlineData("EnAttente")]
    [InlineData("EnCours")]
    [InlineData("Signe")]
    [InlineData("Refuse")]
    public async Task CreateAsync_AccepteTousLesStatutsValides(string statut)
    {
        var entity = CreateDocumentGenere(statutSignature: statut);

        _modeleDocRepoMock.Setup(r => r.ExistsAsync(entity.ModeleDocumentId, It.IsAny<CancellationToken>()))
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
        var entity = CreateDocumentGenere();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateDocumentGenere();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<DocumentGenere>>(),
            "kouroukan.events",
            "entity.updated.documentgenere",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateDocumentGenere();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<DocumentGenere>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiStatutSignatureInvalide()
    {
        var entity = CreateDocumentGenere(statutSignature: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut de signature invalide*");
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
            It.IsAny<EntityDeletedEvent<DocumentGenere>>(),
            "kouroukan.events",
            "entity.deleted.documentgenere",
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
            It.IsAny<EntityDeletedEvent<DocumentGenere>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static DocumentGenere CreateDocumentGenere(
        int id = 1,
        string statutSignature = "EnAttente")
    {
        return new DocumentGenere
        {
            Id = id,
            TypeId = 1,
            ModeleDocumentId = 10,
            EleveId = 5,
            EnseignantId = null,
            DonneesJson = "{\"nom\":\"Test\"}",
            DateGeneration = new DateTime(2025, 6, 15),
            StatutSignature = statutSignature,
            CheminFichier = "/documents/test.pdf",
            UserId = 1
        };
    }
}
