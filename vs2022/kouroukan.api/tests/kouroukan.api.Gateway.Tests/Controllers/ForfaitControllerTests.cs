using Xunit;
using FluentAssertions;
using Kouroukan.Api.Gateway.Controllers;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Kouroukan.Api.Gateway.Tests.Controllers;

public sealed class ForfaitControllerTests
{
    private readonly Mock<IForfaitUserService> _forfaitServiceMock;
    private readonly ForfaitController _sut;

    public ForfaitControllerTests()
    {
        _forfaitServiceMock = new Mock<IForfaitUserService>();
        _sut = new ForfaitController(_forfaitServiceMock.Object);
        SetupAuthenticatedUser(42, 10);
    }

    private void SetupAuthenticatedUser(int userId, int companyId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("companyId", companyId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    // ─── GetStatus ───

    [Fact]
    public async Task GetStatus_DoitRetournerStatut_QuandAuthentifie()
    {
        // Arrange
        var status = new ForfaitStatusDto { EstActif = true, ForfaitNom = "Premium" };
        _forfaitServiceMock
            .Setup(x => x.GetStatusAsync(10, 42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        // Act
        var result = await _sut.GetStatus(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ForfaitStatusDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.EstActif.Should().BeTrue();
    }

    // ─── GetAvailablePlans ───

    [Fact]
    public async Task GetAvailablePlans_DoitRetournerPlans_QuandTypeValide()
    {
        // Arrange
        var plans = new List<ForfaitPlanDto> { new() { Id = 1, Nom = "Essentiel" } };
        _forfaitServiceMock
            .Setup(x => x.GetAvailablePlansAsync("etablissement", It.IsAny<CancellationToken>()))
            .ReturnsAsync(plans);

        // Act
        var result = await _sut.GetAvailablePlans("etablissement", CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<ForfaitPlanDto>>>().Subject;
        response.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAvailablePlans_DoitRetournerBadRequest_QuandTypeVide()
    {
        // Act
        var result = await _sut.GetAvailablePlans("", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAvailablePlans_DoitRetournerBadRequest_QuandTypeEspaces()
    {
        // Act
        var result = await _sut.GetAvailablePlans("   ", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── Subscribe ───

    [Fact]
    public async Task Subscribe_DoitRetournerAbonnement_QuandSouscriptionReussie()
    {
        // Arrange
        var request = new SubscribeForfaitRequest { ForfaitId = 1 };
        var history = new AbonnementHistoryDto { Id = 1, ForfaitNom = "Premium", Statut = "actif" };
        _forfaitServiceMock
            .Setup(x => x.SubscribeAsync(10, 42, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(history);

        // Act
        var result = await _sut.Subscribe(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<AbonnementHistoryDto>>().Subject;
        response.Data!.Statut.Should().Be("actif");
    }

    [Fact]
    public async Task Subscribe_DoitRetournerConflict_QuandAbonnementActifExiste()
    {
        // Arrange
        var request = new SubscribeForfaitRequest { ForfaitId = 1 };
        _forfaitServiceMock
            .Setup(x => x.SubscribeAsync(10, 42, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Un abonnement actif existe deja."));

        // Act
        var result = await _sut.Subscribe(request, CancellationToken.None);

        // Assert
        var conflictResult = result.Should().BeOfType<ConflictObjectResult>().Subject;
        var response = conflictResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Subscribe_DoitRetournerNotFound_QuandForfaitInexistant()
    {
        // Arrange
        var request = new SubscribeForfaitRequest { ForfaitId = 999 };
        _forfaitServiceMock
            .Setup(x => x.SubscribeAsync(10, 42, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Forfait introuvable ou inactif."));

        // Act
        var result = await _sut.Subscribe(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ─── Cancel ───

    [Fact]
    public async Task Cancel_DoitRetournerOk_QuandResiliationReussie()
    {
        // Arrange
        var request = new CancelForfaitRequest { AbonnementId = 1 };
        _forfaitServiceMock
            .Setup(x => x.CancelAsync(1, 10, 42, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Cancel(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Cancel_DoitRetournerNotFound_QuandAbonnementInexistant()
    {
        // Arrange
        var request = new CancelForfaitRequest { AbonnementId = 999 };
        _forfaitServiceMock
            .Setup(x => x.CancelAsync(999, 10, 42, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Abonnement introuvable ou deja resilie."));

        // Act
        var result = await _sut.Cancel(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ─── GetHistory ───

    [Fact]
    public async Task GetHistory_DoitRetournerHistorique()
    {
        // Arrange
        var history = new List<AbonnementHistoryDto>
        {
            new() { Id = 1, ForfaitNom = "Premium", Statut = "actif" },
            new() { Id = 2, ForfaitNom = "Essentiel", Statut = "expire" }
        };
        _forfaitServiceMock
            .Setup(x => x.GetHistoryAsync(10, 42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(history);

        // Act
        var result = await _sut.GetHistory(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<AbonnementHistoryDto>>>().Subject;
        response.Data.Should().HaveCount(2);
    }

    // ─── CheckStudentQuota ───

    [Fact]
    public async Task CheckStudentQuota_DoitRetournerQuota_QuandCompanyIdPresent()
    {
        // Arrange
        var quota = new QuotaCheckResult { LimiteAtteinte = false, Limite = 100, NombreActuel = 50 };
        _forfaitServiceMock
            .Setup(x => x.CheckStudentQuotaAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quota);

        // Act
        var result = await _sut.CheckStudentQuota(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<QuotaCheckResult>>().Subject;
        response.Data!.LimiteAtteinte.Should().BeFalse();
        response.Data.NombreActuel.Should().Be(50);
    }

    [Fact]
    public async Task CheckStudentQuota_DoitRetournerBadRequest_QuandPasDeCompanyId()
    {
        // Arrange - user without companyId claim
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "42") };
        var identity = new ClaimsIdentity(claims, "Test");
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };

        // Act
        var result = await _sut.CheckStudentQuota(CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
