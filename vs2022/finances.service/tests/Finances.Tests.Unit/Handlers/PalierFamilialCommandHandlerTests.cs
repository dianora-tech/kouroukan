using Finances.Application.Commands;
using Finances.Application.Handlers;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour PalierFamilialCommandHandler.
/// </summary>
public sealed class PalierFamilialCommandHandlerTests
{
    private readonly Mock<IPalierFamilialService> _serviceMock;
    private readonly PalierFamilialCommandHandler _sut;

    public PalierFamilialCommandHandlerTests()
    {
        _serviceMock = new Mock<IPalierFamilialService>();
        _sut = new PalierFamilialCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetournePalier()
    {
        var command = new CreatePalierFamilialCommand(CompanyId: 1, RangEnfant: 2, ReductionPourcent: 10m);

        var expected = new PalierFamilial { Id = 42, RangEnfant = 2, ReductionPourcent = 10m };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<PalierFamilial>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<PalierFamilial>(e => e.RangEnfant == 2 && e.ReductionPourcent == 10m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.Handle(new DeletePalierFamilialCommand(1), CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistant()
    {
        _serviceMock.Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _sut.Handle(new DeletePalierFamilialCommand(999), CancellationToken.None);

        result.Should().BeFalse();
    }
}
