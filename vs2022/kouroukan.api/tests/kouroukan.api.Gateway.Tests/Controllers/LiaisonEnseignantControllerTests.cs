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

public sealed class LiaisonEnseignantControllerTests
{
    private readonly Mock<ILiaisonEnseignantService> _liaisonServiceMock;
    private readonly LiaisonEnseignantController _sut;

    public LiaisonEnseignantControllerTests()
    {
        _liaisonServiceMock = new Mock<ILiaisonEnseignantService>();
        _sut = new LiaisonEnseignantController(_liaisonServiceMock.Object);
        SetupAuthenticatedUser(42);
    }

    private void SetupAuthenticatedUser(int userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    // ─── GetLiaisons ───

    [Fact]
    public async Task GetLiaisons_DoitRetournerListe_SansFiltres()
    {
        // Arrange
        var items = new List<LiaisonEnseignantDto>
        {
            new() { Id = 1, UserId = 42, CompanyId = 10, Statut = "en_attente" }
        };
        _liaisonServiceMock
            .Setup(x => x.GetLiaisonsAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.GetLiaisons();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<LiaisonEnseignantDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetLiaisons_DoitFiltrerParUserId()
    {
        // Arrange
        _liaisonServiceMock
            .Setup(x => x.GetLiaisonsAsync(42, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LiaisonEnseignantDto>());

        // Act
        var result = await _sut.GetLiaisons(userId: 42);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _liaisonServiceMock.Verify(x => x.GetLiaisonsAsync(42, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── CreateLiaison ───

    [Fact]
    public async Task CreateLiaison_DoitRetournerLiaisonCreee()
    {
        // Arrange
        var request = new CreateLiaisonEnseignantRequest { CompanyId = 10 };
        var liaison = new LiaisonEnseignantDto { Id = 1, UserId = 42, CompanyId = 10, Statut = "en_attente" };
        _liaisonServiceMock
            .Setup(x => x.CreateLiaisonAsync(42, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(liaison);

        // Act
        var result = await _sut.CreateLiaison(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<LiaisonEnseignantDto>>().Subject;
        response.Data!.Statut.Should().Be("en_attente");
    }

    // ─── AcceptLiaison ───

    [Fact]
    public async Task AcceptLiaison_DoitRetournerOk()
    {
        // Arrange
        _liaisonServiceMock
            .Setup(x => x.AcceptLiaisonAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AcceptLiaison(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _liaisonServiceMock.Verify(x => x.AcceptLiaisonAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── RejectLiaison ───

    [Fact]
    public async Task RejectLiaison_DoitRetournerOk()
    {
        // Arrange
        _liaisonServiceMock
            .Setup(x => x.RejectLiaisonAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.RejectLiaison(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // ─── TerminateLiaison ───

    [Fact]
    public async Task TerminateLiaison_DoitRetournerOk()
    {
        // Arrange
        _liaisonServiceMock
            .Setup(x => x.TerminateLiaisonAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.TerminateLiaison(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // ─── ReintegrateLiaison ───

    [Fact]
    public async Task ReintegrateLiaison_DoitRetournerOk()
    {
        // Arrange
        _liaisonServiceMock
            .Setup(x => x.ReintegrateLiaisonAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.ReintegrateLiaison(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
