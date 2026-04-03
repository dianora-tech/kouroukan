using FluentAssertions;
using GnDapper.Models;
using Communication.Application.Handlers;
using Communication.Application.Queries;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Communication.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour NotificationQueryHandler.
/// </summary>
public sealed class NotificationQueryHandlerTests
{
    private readonly Mock<INotificationService> _serviceMock;
    private readonly NotificationQueryHandler _sut;

    public NotificationQueryHandlerTests()
    {
        _serviceMock = new Mock<INotificationService>();
        _sut = new NotificationQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNotification()
    {
        var notification = new Notification { Id = 1, Contenu = "Test" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var result = await _sut.Handle(new GetNotificationByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        var result = await _sut.Handle(new GetNotificationByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var notifications = new List<Notification>
        {
            new() { Id = 1, Contenu = "Test 1" },
            new() { Id = 2, Contenu = "Test 2" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        var result = await _sut.Handle(new GetAllNotificationsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Notification>(
            new List<Notification> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedNotificationsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
