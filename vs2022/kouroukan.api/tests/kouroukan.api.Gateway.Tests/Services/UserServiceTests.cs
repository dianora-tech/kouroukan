using FluentAssertions;
using GnDapper.Connection;
using GnSecurity.Hashing;
using Kouroukan.Api.Gateway.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Services;

public sealed class UserServiceTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _sut = new UserService(
            _connectionFactoryMock.Object,
            _passwordHasherMock.Object,
            _loggerMock.Object);
    }

    // ─── Construction ───

    [Fact]
    public void Constructeur_DoitCreerInstance_QuandDependancesValides()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Constructeur_DoitImplementerIUserService()
    {
        // Assert
        _sut.Should().BeAssignableTo<IUserService>();
    }

    // ─── Interface ───

    [Fact]
    public void Interface_DoitExposerCreateUserAsync()
    {
        // Assert
        var method = typeof(IUserService).GetMethod("CreateUserAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<Models.CreateUserResultDto>));
    }

    [Fact]
    public void Interface_DoitExposerGetUsersForCompanyAsync()
    {
        // Assert
        var method = typeof(IUserService).GetMethod("GetUsersForCompanyAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerSearchUserAsync()
    {
        // Assert
        var method = typeof(IUserService).GetMethod("SearchUserAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerGetCompaniesForUserAsync()
    {
        // Assert
        var method = typeof(IUserService).GetMethod("GetCompaniesForUserAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerDeleteUserFromCompanyAsync()
    {
        // Assert
        var method = typeof(IUserService).GetMethod("DeleteUserFromCompanyAsync");
        method.Should().NotBeNull();
    }
}
