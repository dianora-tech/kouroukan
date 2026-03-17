using FluentAssertions;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Auth;
using Moq;
using System.Data;

namespace Kouroukan.Api.Gateway.Tests.Auth;

public class RefreshTokenStoreTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;

    public RefreshTokenStoreTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
    }

    [Fact]
    public void Constructor_ValidDependency_CreatesInstance()
    {
        // Act
        var store = new RefreshTokenStore(_connectionFactoryMock.Object);

        // Assert
        store.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ImplementsIRefreshTokenStore()
    {
        // Act
        var store = new RefreshTokenStore(_connectionFactoryMock.Object);

        // Assert
        store.Should().BeAssignableTo<GnSecurity.Jwt.IRefreshTokenStore>();
    }
}
