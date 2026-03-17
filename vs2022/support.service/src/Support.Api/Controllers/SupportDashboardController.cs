using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support.Api.Models;
using Support.Application.Queries;

namespace Support.Api.Controllers;

/// <summary>
/// Controleur pour le dashboard support (admin_it, directeur).
/// </summary>
[ApiController]
[Route("api/support/dashboard")]
[Authorize]
public sealed class SupportDashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SupportDashboardController> _logger;

    public SupportDashboardController(IMediator mediator, ILogger<SupportDashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Recupere les donnees du dashboard support.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:support:manage")]
    public async Task<ActionResult<ApiResponse<SupportDashboardResult>>> GetDashboard(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET support dashboard [CorrelationId: {CorrelationId}]", correlationId);

        var result = await _mediator.Send(new GetSupportDashboardQuery(), ct);
        var response = ApiResponse<SupportDashboardResult>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
