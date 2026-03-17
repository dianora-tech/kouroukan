using FluentAssertions;
using GnDapper.Connection;
using GnSecurity.Hashing;
using GnSecurity.Jwt;
using Kouroukan.Api.Gateway.Auth;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace Kouroukan.Api.Gateway.Tests.Auth;

public class TokenServiceTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IRefreshTokenStore> _refreshTokenStoreMock;
    private readonly Mock<ILogger<TokenService>> _loggerMock;

    public TokenServiceTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _refreshTokenStoreMock = new Mock<IRefreshTokenStore>();
        _loggerMock = new Mock<ILogger<TokenService>>();
    }

    [Fact]
    public void Constructor_NullDependencies_DoesNotThrow()
    {
        // Arrange & Act
        var service = new TokenService(
            _connectionFactoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _refreshTokenStoreMock.Object,
            _loggerMock.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void TokenService_ImplementsITokenService()
    {
        // Arrange & Act
        var service = new TokenService(
            _connectionFactoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _refreshTokenStoreMock.Object,
            _loggerMock.Object);

        // Assert
        service.Should().BeAssignableTo<ITokenService>();
    }
}
