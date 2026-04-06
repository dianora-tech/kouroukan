using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kouroukan.Api.Gateway.Controllers;

/// <summary>
/// Endpoint de rapport de deploiement.
/// Appele par GitHub Actions apres chaque livraison pour envoyer un email recapitulatif.
/// Protege par une cle API (pas par JWT, car appele depuis le CI).
/// </summary>
[ApiController]
[Route("api/deployment")]
[AllowAnonymous]
public sealed class DeploymentController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeploymentController> _logger;

    public DeploymentController(
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<DeploymentController> logger)
    {
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Recoit un rapport de deploiement et envoie un email recapitulatif.
    /// Necessite le header X-Deploy-Key avec la cle configuree.
    /// </summary>
    [HttpPost("report")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendReport([FromBody] DeploymentReportRequest report, CancellationToken ct)
    {
        // Validation de la cle API
        var expectedKey = _configuration["Deployment:ApiKey"];
        if (string.IsNullOrEmpty(expectedKey))
        {
            _logger.LogWarning("Deployment:ApiKey non configuree — endpoint desactive");
            return BadRequest(ApiResponse<object>.Fail("Endpoint non configure."));
        }

        var providedKey = Request.Headers["X-Deploy-Key"].FirstOrDefault();
        if (string.IsNullOrEmpty(providedKey) || providedKey != expectedKey)
        {
            _logger.LogWarning("Tentative d'acces au rapport de deploiement avec une cle invalide");
            return Unauthorized(ApiResponse<object>.Fail("Cle API invalide."));
        }

        // Validation basique
        if (string.IsNullOrWhiteSpace(report.Environment) || string.IsNullOrWhiteSpace(report.Status))
        {
            return BadRequest(ApiResponse<object>.Fail("Les champs 'environment' et 'status' sont requis."));
        }

        // Destinataires configurables
        var recipients = _configuration["Deployment:ReportRecipients"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(recipients))
        {
            _logger.LogWarning("Deployment:ReportRecipients non configure — aucun email envoye");
            return BadRequest(ApiResponse<object>.Fail("Aucun destinataire configure."));
        }

        // Envoyer le rapport a chaque destinataire
        var emails = recipients.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var tasks = emails.Select(email => _emailService.SendDeploymentReportEmailAsync(email.Trim(), report, ct));
        await Task.WhenAll(tasks);

        _logger.LogInformation(
            "Rapport de deploiement {Status} envoye pour {Env} ({Version}) a {Count} destinataire(s)",
            report.Status, report.Environment, report.AppVersion, emails.Length);

        return Ok(ApiResponse<object>.Ok(null!, $"Rapport envoye a {emails.Length} destinataire(s)."));
    }
}
