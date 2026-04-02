using Finances.Api.Models;
using Finances.Application.Commands;
using Finances.Application.Queries;
using Finances.Domain.Entities;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finances.Api.Controllers;

[ApiController]
[Route("api/finances/paliers-familiaux")]
[Authorize]
public sealed class PalierFamilialController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PalierFamilialController> _logger;

    public PalierFamilialController(IMediator mediator, ILogger<PalierFamilialController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere les paliers familiaux avec pagination et filtre par etablissement.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<PalierFamilial>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] int? companyId = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET paliers-familiaux page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedPaliersFamiliauxQuery(page, pageSize, companyId, orderBy), ct);
        var response = ApiResponse<PagedResult<PalierFamilial>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau palier familial.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:finances:create")]
    public async Task<ActionResult<ApiResponse<PalierFamilial>>> Create(
        [FromBody] CreatePalierFamilialCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<PalierFamilial>.Ok(created, "Palier familial cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(null, new { id = created.Id }, response);
    }

    /// <summary>Supprime un palier familial.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeletePalierFamilialCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Palier familial {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Palier familial supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
