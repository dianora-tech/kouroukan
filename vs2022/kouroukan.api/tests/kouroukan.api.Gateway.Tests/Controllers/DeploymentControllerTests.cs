using Xunit;
using FluentAssertions;
using Kouroukan.Api.Gateway.Controllers;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Controllers;

public sealed class DeploymentControllerTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ILogger<DeploymentController>> _loggerMock;
    private readonly DeploymentController _sut;
    private readonly Dictionary<string, string?> _configData;

    public DeploymentControllerTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILogger<DeploymentController>>();
        _configData = new Dictionary<string, string?>
        {
            { "Deployment:ApiKey", "test-deploy-key-123" },
            { "Deployment:ReportRecipients", "admin@kouroukan.dianora.org,dev@kouroukan.dianora.org" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_configData)
            .Build();

        _sut = new DeploymentController(_emailServiceMock.Object, configuration, _loggerMock.Object);
    }

    private void SetDeployKeyHeader(string key)
    {
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _sut.HttpContext.Request.Headers["X-Deploy-Key"] = key;
    }

    private static DeploymentReportRequest CreateValidReport() => new()
    {
        Environment = "test",
        Branch = "develop",
        CommitSha = "abc1234567890",
        CommitMessage = "feat: add deployment reports",
        CommitAuthor = "Ibrahima",
        AppVersion = "v1.2-dev.202604061200",
        Status = "success",
        DeploymentDurationSeconds = 185,
        DeploymentStartedAt = "2026-04-06T12:00:00Z",
        WorkflowRunUrl = "https://github.com/dianora-tech/kouroukan/actions/runs/123",
        HealthChecks = new Dictionary<string, string>
        {
            { "kouroukan.dianora.org", "200" },
            { "app.kouroukan.dianora.org", "200" },
            { "api.kouroukan.dianora.org", "200" }
        },
        Commits = new List<string>
        {
            "abc1234 feat: add deployment reports",
            "def5678 fix: email template"
        },
        ImpactedServices = new List<string> { "Gateway", "Frontend" },
        CodeCoverage = new Dictionary<string, string>
        {
            { "Frontend", "78.5%" },
            { "Gateway", "62.3%" }
        }
    };

    // ─── Succes ───

    [Fact]
    public async Task SendReport_DoitEnvoyerEmailAuxDestinataires_QuandCleValide()
    {
        // Arrange
        SetDeployKeyHeader("test-deploy-key-123");
        var report = CreateValidReport();

        _emailServiceMock
            .Setup(x => x.SendDeploymentReportEmailAsync(It.IsAny<string>(), It.IsAny<DeploymentReportRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.SendReport(report, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        response.Success.Should().BeTrue();

        _emailServiceMock.Verify(
            x => x.SendDeploymentReportEmailAsync("admin@kouroukan.dianora.org", report, It.IsAny<CancellationToken>()),
            Times.Once);
        _emailServiceMock.Verify(
            x => x.SendDeploymentReportEmailAsync("dev@kouroukan.dianora.org", report, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ─── Cle invalide ───

    [Fact]
    public async Task SendReport_DoitRetournerUnauthorized_QuandCleInvalide()
    {
        // Arrange
        SetDeployKeyHeader("wrong-key");
        var report = CreateValidReport();

        // Act
        var result = await _sut.SendReport(report, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        _emailServiceMock.Verify(
            x => x.SendDeploymentReportEmailAsync(It.IsAny<string>(), It.IsAny<DeploymentReportRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SendReport_DoitRetournerUnauthorized_QuandPasDeHeader()
    {
        // Arrange — pas de header
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        var report = CreateValidReport();

        // Act
        var result = await _sut.SendReport(report, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    // ─── Config manquante ───

    [Fact]
    public async Task SendReport_DoitRetournerBadRequest_QuandApiKeyNonConfiguree()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();
        var controller = new DeploymentController(_emailServiceMock.Object, config, _loggerMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["X-Deploy-Key"] = "any-key";

        // Act
        var result = await controller.SendReport(CreateValidReport(), CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task SendReport_DoitRetournerBadRequest_QuandPasDeDestinataires()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Deployment:ApiKey", "test-key" }
            })
            .Build();
        var controller = new DeploymentController(_emailServiceMock.Object, config, _loggerMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.HttpContext.Request.Headers["X-Deploy-Key"] = "test-key";

        // Act
        var result = await controller.SendReport(CreateValidReport(), CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── Validation ───

    [Fact]
    public async Task SendReport_DoitRetournerBadRequest_QuandEnvironmentVide()
    {
        // Arrange
        SetDeployKeyHeader("test-deploy-key-123");
        var report = CreateValidReport();
        report.Environment = "";

        // Act
        var result = await _sut.SendReport(report, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task SendReport_DoitRetournerBadRequest_QuandStatusVide()
    {
        // Arrange
        SetDeployKeyHeader("test-deploy-key-123");
        var report = CreateValidReport();
        report.Status = "";

        // Act
        var result = await _sut.SendReport(report, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
