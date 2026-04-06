using Xunit;
using FluentAssertions;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Controllers;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Kouroukan.Api.Gateway.Tests.Controllers;

public sealed class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly UsersController _sut;

    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _emailServiceMock = new Mock<IEmailService>();
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _sut = new UsersController(
            _userServiceMock.Object,
            _emailServiceMock.Object,
            _connectionFactoryMock.Object);
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

    // ─── List ───

    [Fact]
    public async Task List_DoitRetournerListeUtilisateurs_QuandDirecteurAuthentifie()
    {
        // Arrange
        var users = new List<UserListItemDto>
        {
            new() { Id = 1, FirstName = "Alpha", LastName = "Barry", Role = "enseignant" },
            new() { Id = 2, FirstName = "Mariama", LastName = "Diallo", Role = "censeur" }
        };
        _userServiceMock
            .Setup(x => x.GetUsersForCompanyAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _sut.List(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<UserListItemDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerResultat_QuandCreationReussie()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FirstName = "Ibrahima",
            LastName = "Sow",
            PhoneNumber = "629817970",
            Role = "enseignant"
        };
        var expected = new CreateUserResultDto { UserId = 10, TemporaryPassword = "Abc123!@" };
        _userServiceMock
            .Setup(x => x.CreateUserAsync(42, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<CreateUserResultDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.UserId.Should().Be(10);
        response.Data.TemporaryPassword.Should().Be("Abc123!@");
    }

    // ─── Search ───

    [Fact]
    public async Task Search_DoitRetournerUtilisateur_QuandTrouve()
    {
        // Arrange
        var user = new UserSearchResultDto { Id = 5, FirstName = "Moussa", LastName = "Camara" };
        _userServiceMock
            .Setup(x => x.SearchUserAsync("629000000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.Search("629000000", CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<UserSearchResultDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Id.Should().Be(5);
    }

    [Fact]
    public async Task Search_DoitRetournerNotFound_QuandUtilisateurInexistant()
    {
        // Arrange
        _userServiceMock
            .Setup(x => x.SearchUserAsync("000000000", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserSearchResultDto?)null);

        // Act
        var result = await _sut.Search("000000000", CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Search_DoitRetournerBadRequest_QuandQueryVide()
    {
        // Act
        var result = await _sut.Search("", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Search_DoitRetournerBadRequest_QuandQueryNull()
    {
        // Act
        var result = await _sut.Search("   ", CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── Companies ───

    [Fact]
    public async Task Companies_DoitRetournerEtablissements_QuandUtilisateurAuthentifie()
    {
        // Arrange
        var companies = new List<CompanyDto>
        {
            new() { Id = 1, Name = "Ecole Alpha", Role = "owner" }
        };
        _userServiceMock
            .Setup(x => x.GetCompaniesForUserAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(companies);

        // Act
        var result = await _sut.Companies(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<CompanyDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(1);
    }

    // ─── Delete ───

    [Fact]
    public async Task Delete_DoitRetournerOk_QuandSuppressionReussie()
    {
        // Arrange
        _userServiceMock
            .Setup(x => x.DeleteUserFromCompanyAsync(42, 10, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Delete(10, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeTrue();
        _userServiceMock.Verify(x => x.DeleteUserFromCompanyAsync(42, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
