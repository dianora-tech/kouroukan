using GnDapper.Models;
using Inscriptions.Api.Models;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inscriptions.Api.Controllers;

[ApiController]
[Route("api/inscriptions/liaisons-parent")]
[Authorize]
public sealed class LiaisonParentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LiaisonParentController> _logger;

    public LiaisonParentController(IMediator mediator, ILogger<LiaisonParentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere les liaisons parent avec pagination et filtres.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<LiaisonParent>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] int? parentUserId = null, [FromQuery] int? companyId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET liaisons-parent page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedLiaisonsParentQuery(page, pageSize, parentUserId, companyId, orderBy), ct);
        var response = ApiResponse<PagedResult<LiaisonParent>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle liaison parent (scan QR par l'etablissement).</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:inscriptions:create")]
    public async Task<ActionResult<ApiResponse<LiaisonParent>>> Create(
        [FromBody] CreateLiaisonParentCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<LiaisonParent>.Ok(created, "Liaison parent creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(null, new { id = created.Id }, response);
    }

    /// <summary>Supprime une liaison parent.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteLiaisonParentCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Liaison parent {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Liaison parent supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
