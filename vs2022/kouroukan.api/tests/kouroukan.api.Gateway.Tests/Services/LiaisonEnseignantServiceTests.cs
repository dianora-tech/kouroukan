using FluentAssertions;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Services;

public sealed class LiaisonEnseignantServiceTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly Mock<ILogger<LiaisonEnseignantService>> _loggerMock;
    private readonly LiaisonEnseignantService _sut;

    public LiaisonEnseignantServiceTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _loggerMock = new Mock<ILogger<LiaisonEnseignantService>>();
        _sut = new LiaisonEnseignantService(_connectionFactoryMock.Object, _loggerMock.Object);
    }

    // ─── Construction ───

    [Fact]
    public void Constructeur_DoitCreerInstance_QuandDependancesValides()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Constructeur_DoitImplementerILiaisonEnseignantService()
    {
        // Assert
        _sut.Should().BeAssignableTo<ILiaisonEnseignantService>();
    }

    // ─── Interface ───

    [Fact]
    public void Interface_DoitExposerGetLiaisonsAsync()
    {
        var method = typeof(ILiaisonEnseignantService).GetMethod("GetLiaisonsAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<List<LiaisonEnseignantDto>>));
    }

    [Fact]
    public void Interface_DoitExposerCreateLiaisonAsync()
    {
        var method = typeof(ILiaisonEnseignantService).GetMethod("CreateLiaisonAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<LiaisonEnseignantDto>));
    }

    [Fact]
    public void Interface_DoitExposerAcceptLiaisonAsync()
    {
        var method = typeof(ILiaisonEnseignantService).GetMethod("AcceptLiaisonAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }

    [Fact]
    public void Interface_DoitExposerRejectLiaisonAsync()
    {
        var method = typeof(ILiaisonEnseignantService).GetMethod("RejectLiaisonAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerTerminateLiaisonAsync()
    {
        var method = typeof(ILiaisonEnseignantService).GetMethod("TerminateLiaisonAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerReintegrateLiaisonAsync()
    {
        var method = typeof(ILiaisonEnseignantService).GetMethod("ReintegrateLiaisonAsync");
        method.Should().NotBeNull();
    }
}
