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
/// Tests unitaires pour NotificationService.
/// </summary>
public sealed class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<NotificationService>> _loggerMock;
    private readonly NotificationService _sut;

    public NotificationServiceTests()
    {
        _repoMock = new Mock<INotificationRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<NotificationService>>();

        _sut = new NotificationService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneNotification_QuandExiste()
    {
        var notification = CreateNotification();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var notifications = new List<Notification>
        {
            CreateNotification(),
            CreateNotification(id: 2)
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Notification>(
            new List<Notification> { CreateNotification() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLaNotification_AvecDonneesValides()
    {
        var entity = CreateNotification();

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
        var entity = CreateNotification();

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Notification>>(),
            "kouroukan.events",
            "entity.created.notification",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiContenuVide()
    {
        var entity = CreateNotification();
        entity.Contenu = "";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*contenu de la notification est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiContenuEspaces()
    {
        var entity = CreateNotification();
        entity.Contenu = "   ";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*contenu de la notification est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCanalVide()
    {
        var entity = CreateNotification();
        entity.Canal = "";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*canal de la notification est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCanalEspaces()
    {
        var entity = CreateNotification();
        entity.Canal = "   ";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*canal de la notification est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiCanalInvalide()
    {
        var entity = CreateNotification();
        entity.Canal = "Fax";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*canal*n'est pas valide*");
    }

    [Theory]
    [InlineData("Push")]
    [InlineData("SMS")]
    [InlineData("Email")]
    [InlineData("InApp")]
    public async Task CreateAsync_AccepteTousLesCanauxValides(string canal)
    {
        var entity = CreateNotification();
        entity.Canal = canal;

        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateNotification();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateNotification();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Notification>>(),
            "kouroukan.events",
            "entity.updated.notification",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateNotification();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Notification>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_RetourneFalse_QuandInexistante()
    {
        var entity = CreateNotification();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeFalse();
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
            It.IsAny<EntityDeletedEvent<Notification>>(),
            "kouroukan.events",
            "entity.deleted.notification",
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
            It.IsAny<EntityDeletedEvent<Notification>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static Notification CreateNotification(int id = 1)
    {
        return new Notification
        {
            Id = id,
            TypeId = 1,
            DestinatairesIds = "[1, 2, 3]",
            Contenu = "Contenu de la notification de test",
            Canal = "Push",
            EstEnvoyee = false,
            DateEnvoi = null,
            LienAction = null,
            UserId = 1
        };
    }
}
