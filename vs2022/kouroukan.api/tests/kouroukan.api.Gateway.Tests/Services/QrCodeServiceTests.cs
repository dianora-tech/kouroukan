using FluentAssertions;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Services;

public sealed class QrCodeServiceTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly Mock<ILogger<QrCodeService>> _loggerMock;
    private readonly QrCodeService _sut;

    public QrCodeServiceTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _loggerMock = new Mock<ILogger<QrCodeService>>();
        _sut = new QrCodeService(_connectionFactoryMock.Object, _loggerMock.Object);
    }

    // ─── Construction ───

    [Fact]
    public void Constructeur_DoitCreerInstance_QuandDependancesValides()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Constructeur_DoitImplementerIQrCodeService()
    {
        // Assert
        _sut.Should().BeAssignableTo<IQrCodeService>();
    }

    // ─── Interface ───

    [Fact]
    public void Interface_DoitExposerGetOrCreateQrCodeAsync()
    {
        var method = typeof(IQrCodeService).GetMethod("GetOrCreateQrCodeAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<QrCodeDto>));
    }

    [Fact]
    public void Interface_DoitExposerResolveQrCodeAsync()
    {
        var method = typeof(IQrCodeService).GetMethod("ResolveQrCodeAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<QrCodeResolvedDto?>));
    }
}
