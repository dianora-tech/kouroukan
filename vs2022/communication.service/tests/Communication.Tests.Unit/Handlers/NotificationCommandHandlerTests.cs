using FluentAssertions;
using Communication.Application.Commands;
using Communication.Application.Handlers;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Communication.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour NotificationCommandHandler.
/// </summary>
public sealed class NotificationCommandHandlerTests
{
    private readonly Mock<INotificationService> _serviceMock;
    private readonly NotificationCommandHandler _sut;

    public NotificationCommandHandlerTests()
    {
        _serviceMock = new Mock<INotificationService>();
        _sut = new NotificationCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneNotification()
    {
        var command = new CreateNotificationCommand(
            TypeId: 1,
            DestinatairesIds: "[1, 2, 3]",
            Contenu: "Contenu test",
            Canal: "Push",
            EstEnvoyee: false,
            DateEnvoi: null,
            LienAction: null,
            UserId: 1);

        var expected = new Notification
        {
            Id = 42,
            TypeId = 1,
            Contenu = "Contenu test",
            Canal = "Push"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Notification>(n =>
                n.TypeId == 1 &&
                n.DestinatairesIds == "[1, 2, 3]" &&
                n.Contenu == "Contenu test" &&
                n.Canal == "Push" &&
                n.UserId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateNotificationCommand(
            Id: 1,
            TypeId: 1,
            DestinatairesIds: "[1, 2]",
            Contenu: "Contenu modifie",
            Canal: "Email",
            EstEnvoyee: true,
            DateEnvoi: DateTime.Now,
            LienAction: "https://example.com",
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Notification>(n => n.Id == 1 && n.EstEnvoyee),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateNotificationCommand(
            Id: 999, TypeId: 1, DestinatairesIds: "[1]", Contenu: "Test",
            Canal: "Push", EstEnvoyee: false, DateEnvoi: null,
            LienAction: null, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteNotificationCommand(1);

        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistante()
    {
        var command = new DeleteNotificationCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
