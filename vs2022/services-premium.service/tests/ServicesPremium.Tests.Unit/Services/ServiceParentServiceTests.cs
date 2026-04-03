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
/// Tests unitaires pour ServiceParentService.
/// </summary>
public sealed class ServiceParentServiceTests
{
    private readonly Mock<IServiceParentRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<ServiceParentService>> _loggerMock;
    private readonly ServiceParentService _sut;

    public ServiceParentServiceTests()
    {
        _repoMock = new Mock<IServiceParentRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<ServiceParentService>>();

        _sut = new ServiceParentService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_RetourneServiceParent_QuandExiste()
    {
        var entity = CreateServiceParent();
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
            .ReturnsAsync((ServiceParent?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var list = new List<ServiceParent> { CreateServiceParent(), CreateServiceParent(id: 2) };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // --- GetPagedAsync ---

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<ServiceParent>(
            new List<ServiceParent> { CreateServiceParent() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_CreeLeServiceParent_AvecPeriodiciteValide()
    {
        var entity = CreateServiceParent();
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
        var entity = CreateServiceParent();
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<ServiceParent>>(),
            "kouroukan.events",
            "entity.created.serviceparent",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiPeriodiciteInvalide()
    {
        var entity = CreateServiceParent(periodicite: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Periodicite invalide*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiTarifNegatif()
    {
        var entity = CreateServiceParent(tarif: -100m);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*tarif ne peut pas etre negatif*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiPeriodeEssaiNegative()
    {
        var entity = CreateServiceParent(periodeEssaiJours: -5);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*periode d'essai ne peut pas etre negative*");
    }

    [Theory]
    [InlineData("Mensuel")]
    [InlineData("Annuel")]
    [InlineData("Unite")]
    public async Task CreateAsync_AccepteToutesLesPeriodicites(string periodicite)
    {
        var entity = CreateServiceParent(periodicite: periodicite);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_TarifZero_Accepte()
    {
        var entity = CreateServiceParent(tarif: 0m);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateServiceParent();
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateServiceParent();
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<ServiceParent>>(),
            "kouroukan.events",
            "entity.updated.serviceparent",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateServiceParent();
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<ServiceParent>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Lance_KeyNotFoundException_SiInexistant()
    {
        var entity = CreateServiceParent();
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*ServiceParent*introuvable*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiPeriodiciteInvalide()
    {
        var entity = CreateServiceParent(periodicite: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Periodicite invalide*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiTarifNegatif()
    {
        var entity = CreateServiceParent(tarif: -50m);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*tarif ne peut pas etre negatif*");
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
            It.IsAny<EntityDeletedEvent<ServiceParent>>(),
            "kouroukan.events",
            "entity.deleted.serviceparent",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Lance_KeyNotFoundException_SiInexistant()
    {
        _repoMock.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _sut.DeleteAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*ServiceParent*introuvable*");
    }

    // --- Helper ---

    private static ServiceParent CreateServiceParent(
        int id = 1,
        string periodicite = "Mensuel",
        decimal tarif = 50000m,
        int? periodeEssaiJours = null)
    {
        return new ServiceParent
        {
            Id = id,
            TypeId = 1,
            Name = "Service SMS",
            Description = "Alertes SMS aux parents",
            Code = "SVC-SMS-001",
            Tarif = tarif,
            Periodicite = periodicite,
            EstActif = true,
            PeriodeEssaiJours = periodeEssaiJours,
            TarifDegressif = false,
            UserId = 1
        };
    }
}
