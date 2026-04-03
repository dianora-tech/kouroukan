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

public sealed class QrCodeControllerTests
{
    private readonly Mock<IQrCodeService> _qrCodeServiceMock;
    private readonly QrCodeController _sut;

    public QrCodeControllerTests()
    {
        _qrCodeServiceMock = new Mock<IQrCodeService>();
        _sut = new QrCodeController(_qrCodeServiceMock.Object);
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

    // ─── GetMyQrCode ───

    [Fact]
    public async Task GetMyQrCode_DoitRetournerQrCode_QuandAuthentifie()
    {
        // Arrange
        var qrCode = new QrCodeDto { Id = 1, UserId = 42, Code = "ABC123DEF456" };
        _qrCodeServiceMock
            .Setup(x => x.GetOrCreateQrCodeAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(qrCode);

        // Act
        var result = await _sut.GetMyQrCode(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<QrCodeDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Code.Should().Be("ABC123DEF456");
        response.Data.UserId.Should().Be(42);
    }

    // ─── ResolveQrCode ───

    [Fact]
    public async Task ResolveQrCode_DoitRetournerProfil_QuandCodeValide()
    {
        // Arrange
        var resolved = new QrCodeResolvedDto
        {
            UserId = 10,
            FirstName = "Alpha",
            LastName = "Barry",
            Role = "enseignant"
        };
        _qrCodeServiceMock
            .Setup(x => x.ResolveQrCodeAsync("VALIDCODE123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(resolved);

        // Act
        var result = await _sut.ResolveQrCode("VALIDCODE123", CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<QrCodeResolvedDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.UserId.Should().Be(10);
        response.Data.FirstName.Should().Be("Alpha");
    }

    [Fact]
    public async Task ResolveQrCode_DoitRetournerNotFound_QuandCodeInvalide()
    {
        // Arrange
        _qrCodeServiceMock
            .Setup(x => x.ResolveQrCodeAsync("INVALIDCODE", It.IsAny<CancellationToken>()))
            .ReturnsAsync((QrCodeResolvedDto?)null);

        // Act
        var result = await _sut.ResolveQrCode("INVALIDCODE", CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
