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
/// Tests unitaires pour SignatureService.
/// </summary>
public sealed class SignatureServiceTests
{
    private readonly Mock<ISignatureRepository> _repoMock;
    private readonly Mock<IDocumentGenereRepository> _docGenRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<SignatureService>> _loggerMock;
    private readonly SignatureService _sut;

    public SignatureServiceTests()
    {
        _repoMock = new Mock<ISignatureRepository>();
        _docGenRepoMock = new Mock<IDocumentGenereRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<SignatureService>>();

        _sut = new SignatureService(
            _repoMock.Object,
            _docGenRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneSignature_QuandExiste()
    {
        var entity = CreateSignature();
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
            .ReturnsAsync((Signature?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var entities = new List<Signature> { CreateSignature(), CreateSignature(id: 2) };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Signature>(
            new List<Signature> { CreateSignature() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeSignature_AvecDonneesValides()
    {
        var entity = CreateSignature();

        _docGenRepoMock.Setup(r => r.ExistsAsync(entity.DocumentGenereId, It.IsAny<CancellationToken>()))
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
        var entity = CreateSignature();

        _docGenRepoMock.Setup(r => r.ExistsAsync(entity.DocumentGenereId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Signature>>(),
            "kouroukan.events",
            "entity.created.signature",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiStatutSignatureInvalide()
    {
        var entity = CreateSignature(statutSignature: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut de signature invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiNiveauSignatureInvalide()
    {
        var entity = CreateSignature(niveauSignature: "Expert");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Niveau de signature invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiDocumentGenereInexistant()
    {
        var entity = CreateSignature();

        _docGenRepoMock.Setup(r => r.ExistsAsync(entity.DocumentGenereId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*document genere*n'existe pas*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiOrdreSignatureZero()
    {
        var entity = CreateSignature();
        entity.OrdreSignature = 0;

        _docGenRepoMock.Setup(r => r.ExistsAsync(entity.DocumentGenereId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ordre de signature*superieur a 0*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiOrdreSignatureNegatif()
    {
        var entity = CreateSignature();
        entity.OrdreSignature = -1;

        _docGenRepoMock.Setup(r => r.ExistsAsync(entity.DocumentGenereId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ordre de signature*superieur a 0*");
    }

    [Theory]
    [InlineData("EnAttente")]
    [InlineData("Signe")]
    [InlineData("Refuse")]
    [InlineData("Delegue")]
    public async Task CreateAsync_AccepteTousLesStatutsValides(string statut)
    {
        var entity = CreateSignature(statutSignature: statut);

        _docGenRepoMock.Setup(r => r.ExistsAsync(entity.DocumentGenereId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("Basique")]
    [InlineData("Avancee")]
    public async Task CreateAsync_AccepteTousLesNiveauxValides(string niveau)
    {
        var entity = CreateSignature(niveauSignature: niveau);

        _docGenRepoMock.Setup(r => r.ExistsAsync(entity.DocumentGenereId, It.IsAny<CancellationToken>()))
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
        var entity = CreateSignature();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateSignature();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Signature>>(),
            "kouroukan.events",
            "entity.updated.signature",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateSignature();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Signature>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiStatutSignatureInvalide()
    {
        var entity = CreateSignature(statutSignature: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut de signature invalide*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiNiveauSignatureInvalide()
    {
        var entity = CreateSignature(niveauSignature: "Expert");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Niveau de signature invalide*");
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
            It.IsAny<EntityDeletedEvent<Signature>>(),
            "kouroukan.events",
            "entity.deleted.signature",
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
            It.IsAny<EntityDeletedEvent<Signature>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static Signature CreateSignature(
        int id = 1,
        string statutSignature = "EnAttente",
        string niveauSignature = "Basique")
    {
        return new Signature
        {
            Id = id,
            TypeId = 1,
            DocumentGenereId = 10,
            SignataireId = 5,
            OrdreSignature = 1,
            DateSignature = null,
            StatutSignature = statutSignature,
            NiveauSignature = niveauSignature,
            MotifRefus = null,
            EstValidee = false,
            UserId = 1
        };
    }
}
