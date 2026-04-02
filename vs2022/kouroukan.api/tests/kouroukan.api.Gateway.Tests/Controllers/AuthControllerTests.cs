using FluentAssertions;
using GnDapper.Connection;
using GnSecurity.Jwt;
using Kouroukan.Api.Gateway.Auth;
using Kouroukan.Api.Gateway.Controllers;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace Kouroukan.Api.Gateway.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<IMinioStorageService> _storageServiceMock;
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _storageServiceMock = new Mock<IMinioStorageService>();
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _loggerMock = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(
            _tokenServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _storageServiceMock.Object,
            _connectionFactoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@kouroukan.gn", Password = "password123" };
        var expectedTokens = new AuthTokensDto
        {
            AccessToken = "jwt-token",
            RefreshToken = "refresh-token",
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _tokenServiceMock.Setup(x => x.LoginAsync(request.Email, request.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTokens);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<AuthTokensDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.AccessToken.Should().Be("jwt-token");
        response.Data.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        var request = new RefreshRequest { RefreshToken = "valid-refresh-token" };
        var tokenResult = new TokenResult(
            "new-jwt-token",
            "new-refresh-token",
            DateTime.UtcNow.AddMinutes(15),
            DateTime.UtcNow.AddDays(7));

        _refreshTokenServiceMock.Setup(x => x.RefreshAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResult);

        // Act
        var result = await _controller.Refresh(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<AuthTokensDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.AccessToken.Should().Be("new-jwt-token");
    }

    [Fact]
    public async Task Logout_Authenticated_ReturnsOk()
    {
        // Arrange
        var request = new LogoutRequest { RefreshToken = "token-to-revoke" };
        SetupAuthenticatedUser(1);

        _refreshTokenServiceMock.Setup(x => x.RevokeAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Logout(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeTrue();

        _refreshTokenServiceMock.Verify(x => x.RevokeAsync(request.RefreshToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Me_AuthenticatedUser_ReturnsProfile()
    {
        // Arrange
        SetupAuthenticatedUser(42);

        var expectedProfile = new UserProfileDto
        {
            Id = 42,
            FirstName = "Ibrahima",
            LastName = "Diallo",
            Email = "ibrahima@kouroukan.gn",
            Roles = new List<string> { "directeur" },
            Permissions = new List<string> { "inscriptions:read", "inscriptions:create" },
            CguVersion = "1.0"
        };

        _tokenServiceMock.Setup(x => x.GetUserProfileAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _controller.Me(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<UserProfileDto>>().Subject;
        response.Data!.Id.Should().Be(42);
        response.Data.FirstName.Should().Be("Ibrahima");
    }

    [Fact]
    public async Task Me_NoSubClaim_ReturnsUnauthorized()
    {
        // Arrange - pas de claims
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };

        // Act
        var result = await _controller.Me(CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetActiveCgu_CguExists_ReturnsCgu()
    {
        // Arrange
        var cgu = new CguVersionDto
        {
            Version = "1.0",
            Contenu = "# CGU Kouroukan",
            DatePublication = DateTime.UtcNow
        };

        _tokenServiceMock.Setup(x => x.GetActiveCguAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cgu);

        // Act
        var result = await _controller.GetActiveCgu(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<CguVersionDto>>().Subject;
        response.Data!.Version.Should().Be("1.0");
    }

    [Fact]
    public async Task GetActiveCgu_NoCgu_ReturnsNotFound()
    {
        // Arrange
        _tokenServiceMock.Setup(x => x.GetActiveCguAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CguVersionDto?)null);

        // Act
        var result = await _controller.GetActiveCgu(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task AcceptCgu_Authenticated_ReturnsNewTokens()
    {
        // Arrange
        SetupAuthenticatedUser(42);

        var activeCgu = new CguVersionDto { Version = "1.0", Contenu = "# CGU", DatePublication = DateTime.UtcNow };
        var newTokens = new AuthTokensDto
        {
            AccessToken = "new-token-with-cgu",
            RefreshToken = "new-refresh",
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _tokenServiceMock.Setup(x => x.GetActiveCguAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeCgu);
        _tokenServiceMock.Setup(x => x.AcceptCguAsync(42, "1.0", It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTokens);

        // Act
        var result = await _controller.AcceptCgu(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<AuthTokensDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.AccessToken.Should().Be("new-token-with-cgu");
    }

    private void SetupAuthenticatedUser(int userId)
    {
        var claims = new[]
        {
            new Claim("sub", userId.ToString()),
            new Claim("email", "user@kouroukan.gn"),
            new Claim("name", "Test User")
        };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}
