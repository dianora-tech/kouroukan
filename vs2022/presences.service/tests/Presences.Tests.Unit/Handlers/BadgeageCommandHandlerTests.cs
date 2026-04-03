using FluentAssertions;
using Presences.Application.Commands;
using Presences.Application.Handlers;
using Presences.Domain.Entities;
using Presences.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Presences.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour BadgeageCommandHandler.
/// </summary>
public sealed class BadgeageCommandHandlerTests
{
    private readonly Mock<IBadgeageService> _serviceMock;
    private readonly BadgeageCommandHandler _sut;

    public BadgeageCommandHandlerTests()
    {
        _serviceMock = new Mock<IBadgeageService>();
        _sut = new BadgeageCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneBadgeage()
    {
        var command = new CreateBadgeageCommand(
            TypeId: 1,
            EleveId: 10,
            DateBadgeage: new DateTime(2025, 9, 15),
            HeureBadgeage: new TimeSpan(7, 45, 0),
            PointAcces: "Entree",
            MethodeBadgeage: "NFC",
            UserId: 1);

        var expected = new Badgeage
        {
            Id = 42,
            TypeId = 1,
            EleveId = 10,
            PointAcces = "Entree",
            MethodeBadgeage = "NFC"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Badgeage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Badgeage>(b =>
                b.TypeId == 1 &&
                b.EleveId == 10 &&
                b.PointAcces == "Entree" &&
                b.MethodeBadgeage == "NFC"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateBadgeageCommand(
            Id: 1,
            TypeId: 1,
            EleveId: 10,
            DateBadgeage: new DateTime(2025, 9, 15),
            HeureBadgeage: new TimeSpan(7, 45, 0),
            PointAcces: "Sortie",
            MethodeBadgeage: "QRCode",
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Badgeage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Badgeage>(b => b.Id == 1 && b.PointAcces == "Sortie"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateBadgeageCommand(
            Id: 999, TypeId: 1, EleveId: 10,
            DateBadgeage: DateTime.Today, HeureBadgeage: new TimeSpan(8, 0, 0),
            PointAcces: "Entree", MethodeBadgeage: "NFC", UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Badgeage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteBadgeageCommand(1);

        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistant()
    {
        var command = new DeleteBadgeageCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
