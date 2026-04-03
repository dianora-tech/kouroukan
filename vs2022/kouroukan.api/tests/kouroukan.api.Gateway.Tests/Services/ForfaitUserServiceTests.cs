using FluentAssertions;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Services;

public sealed class ForfaitUserServiceTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly Mock<ILogger<ForfaitUserService>> _loggerMock;
    private readonly ForfaitUserService _sut;

    public ForfaitUserServiceTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _loggerMock = new Mock<ILogger<ForfaitUserService>>();
        _sut = new ForfaitUserService(_connectionFactoryMock.Object, _loggerMock.Object);
    }

    // ─── Construction ───

    [Fact]
    public void Constructeur_DoitCreerInstance_QuandDependancesValides()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Constructeur_DoitImplementerIForfaitUserService()
    {
        // Assert
        _sut.Should().BeAssignableTo<IForfaitUserService>();
    }

    // ─── Interface ───

    [Fact]
    public void Interface_DoitExposerGetStatusAsync()
    {
        var method = typeof(IForfaitUserService).GetMethod("GetStatusAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<ForfaitStatusDto>));
    }

    [Fact]
    public void Interface_DoitExposerGetAvailablePlansAsync()
    {
        var method = typeof(IForfaitUserService).GetMethod("GetAvailablePlansAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<List<ForfaitPlanDto>>));
    }

    [Fact]
    public void Interface_DoitExposerSubscribeAsync()
    {
        var method = typeof(IForfaitUserService).GetMethod("SubscribeAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<AbonnementHistoryDto>));
    }

    [Fact]
    public void Interface_DoitExposerCancelAsync()
    {
        var method = typeof(IForfaitUserService).GetMethod("CancelAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }

    [Fact]
    public void Interface_DoitExposerGetHistoryAsync()
    {
        var method = typeof(IForfaitUserService).GetMethod("GetHistoryAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<List<AbonnementHistoryDto>>));
    }

    [Fact]
    public void Interface_DoitExposerCheckStudentQuotaAsync()
    {
        var method = typeof(IForfaitUserService).GetMethod("CheckStudentQuotaAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<QuotaCheckResult>));
    }
}
