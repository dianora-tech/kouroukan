using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Microsoft.Extensions.Logging;
using Moq;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Output;
using ServicesPremium.Domain.Services;
using Xunit;

namespace ServicesPremium.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour SouscriptionService.
/// </summary>
public sealed class SouscriptionServiceTests
{
    private readonly Mock<ISouscriptionRepository> _repoMock;
    private readonly Mock<IServiceParentRepository> _serviceParentRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<SouscriptionService>> _loggerMock;
    private readonly SouscriptionService _sut;

    public SouscriptionServiceTests()
    {
        _repoMock = new Mock<ISouscriptionRepository>();
        _serviceParentRepoMock = new Mock<IServiceParentRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<SouscriptionService>>();

        _sut = new SouscriptionService(
            _repoMock.Object,
            _serviceParentRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_RetourneSouscription_QuandExiste()
    {
        var entity = CreateSouscription();
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
            .ReturnsAsync((Souscription?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // --- GetPagedAsync ---

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Souscription>(
            new List<Souscription> { CreateSouscription() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_CreeLaSouscription_AvecStatutValide()
    {
        var entity = CreateSouscription();
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
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
        var entity = CreateSouscription();
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Souscription>>(),
            "kouroukan.events",
            "entity.created.souscription",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateSouscription(statut: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiServiceParentInexistant()
    {
        var entity = CreateSouscription();
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*ServiceParent*introuvable*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMontantNegatif()
    {
        var entity = CreateSouscription(montant: -100m);

        // statut valide mais montant negatif
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant paye ne peut pas etre negatif*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiDateFinAnterieureADateDebut()
    {
        var entity = CreateSouscription();
        entity.DateFin = entity.DateDebut.AddDays(-10);

        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*date de fin ne peut pas etre anterieure*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiDateRenouvellementAnterieureADateDebut()
    {
        var entity = CreateSouscription();
        entity.DateProchainRenouvellement = entity.DateDebut.AddDays(-1);

        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*date de prochain renouvellement ne peut pas etre anterieure*");
    }

    [Theory]
    [InlineData("Active")]
    [InlineData("Expiree")]
    [InlineData("Resiliee")]
    [InlineData("Essai")]
    public async Task CreateAsync_AccepteTousLesStatutsValides(string statut)
    {
        var entity = CreateSouscription(statut: statut);
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateSouscription();
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateSouscription();
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Souscription>>(),
            "kouroukan.events",
            "entity.updated.souscription",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateSouscription();
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Souscription>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_KeyNotFoundException_SiInexistante()
    {
        var entity = CreateSouscription();
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Souscription*introuvable*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_KeyNotFoundException_SiServiceParentInexistant()
    {
        var entity = CreateSouscription();
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*ServiceParent*introuvable*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateSouscription(statut: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Statut invalide*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiDateFinAnterieureADateDebut()
    {
        var entity = CreateSouscription();
        entity.DateFin = entity.DateDebut.AddDays(-5);

        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceParentRepoMock.Setup(r => r.ExistsAsync(entity.ServiceParentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*date de fin ne peut pas etre anterieure*");
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_RetourneTrue_QuandSuppressionReussie()
    {
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.DeleteAsync(1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_PublieEvenement_SiReussite()
    {
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.DeleteAsync(1);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityDeletedEvent<Souscription>>(),
            "kouroukan.events",
            "entity.deleted.souscription",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Lance_KeyNotFoundException_SiInexistante()
    {
        _repoMock.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.DeleteAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Souscription*introuvable*");
    }

    // --- Helper ---

    private static Souscription CreateSouscription(
        string statut = "Active",
        decimal montant = 50000m)
    {
        return new Souscription
        {
            Id = 1,
            Name = "Souscription SMS",
            Description = "Souscription au service SMS",
            ServiceParentId = 10,
            ParentId = 5,
            DateDebut = new DateTime(2025, 9, 1),
            DateFin = new DateTime(2026, 8, 31),
            StatutSouscription = statut,
            MontantPaye = montant,
            RenouvellementAuto = true,
            DateProchainRenouvellement = new DateTime(2026, 9, 1),
            UserId = 1
        };
    }
}
