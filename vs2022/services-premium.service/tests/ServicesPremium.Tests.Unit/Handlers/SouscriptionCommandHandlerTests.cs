using FluentAssertions;
using Moq;
using ServicesPremium.Application.Commands;
using ServicesPremium.Application.Handlers;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Input;
using Xunit;

namespace ServicesPremium.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour SouscriptionCommandHandler.
/// </summary>
public sealed class SouscriptionCommandHandlerTests
{
    private readonly Mock<ISouscriptionService> _serviceMock;
    private readonly SouscriptionCommandHandler _sut;

    public SouscriptionCommandHandlerTests()
    {
        _serviceMock = new Mock<ISouscriptionService>();
        _sut = new SouscriptionCommandHandler(_serviceMock.Object);
    }

    // --- Create ---

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneSouscription()
    {
        var command = new CreateSouscriptionCommand(
            Name: "Souscription SMS",
            Description: null,
            ServiceParentId: 10,
            ParentId: 5,
            DateDebut: new DateTime(2025, 9, 1),
            DateFin: new DateTime(2026, 8, 31),
            StatutSouscription: "Active",
            MontantPaye: 50000m,
            RenouvellementAuto: true,
            DateProchainRenouvellement: new DateTime(2026, 9, 1),
            UserId: 1);

        var expected = new Souscription
        {
            Id = 42,
            ServiceParentId = 10,
            ParentId = 5,
            StatutSouscription = "Active"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Souscription>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Souscription>(e =>
                e.ServiceParentId == 10 &&
                e.ParentId == 5 &&
                e.MontantPaye == 50000m &&
                e.StatutSouscription == "Active"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // --- Update ---

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateSouscriptionCommand(
            Id: 1,
            Name: "Souscription SMS v2",
            Description: null,
            ServiceParentId: 10,
            ParentId: 5,
            DateDebut: new DateTime(2025, 9, 1),
            DateFin: null,
            StatutSouscription: "Resiliee",
            MontantPaye: 0m,
            RenouvellementAuto: false,
            DateProchainRenouvellement: null,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Souscription>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Souscription>(e => e.Id == 1 && e.StatutSouscription == "Resiliee"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateSouscriptionCommand(
            Id: 999, Name: "N", Description: null, ServiceParentId: 1, ParentId: 1,
            DateDebut: DateTime.Today, DateFin: null, StatutSouscription: "Active",
            MontantPaye: 0, RenouvellementAuto: false, DateProchainRenouvellement: null, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Souscription>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // --- Delete ---

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteSouscriptionCommand(1);

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
        var command = new DeleteSouscriptionCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
