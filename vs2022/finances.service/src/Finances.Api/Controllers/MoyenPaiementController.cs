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
[Route("api/finances/moyens-paiement")]
[Authorize]
public sealed class MoyenPaiementController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MoyenPaiementController> _logger;

    public MoyenPaiementController(IMediator mediator, ILogger<MoyenPaiementController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere les moyens de paiement avec pagination et filtre par etablissement.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<MoyenPaiement>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] int? companyId = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET moyens-paiement page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedMoyensPaiementQuery(page, pageSize, companyId, orderBy), ct);
        var response = ApiResponse<PagedResult<MoyenPaiement>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau moyen de paiement.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:finances:create")]
    public async Task<ActionResult<ApiResponse<MoyenPaiement>>> Create(
        [FromBody] CreateMoyenPaiementCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<MoyenPaiement>.Ok(created, "Moyen de paiement cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(null, new { id = created.Id }, response);
    }

    /// <summary>Met a jour un moyen de paiement.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateMoyenPaiementCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Moyen de paiement {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Moyen de paiement mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un moyen de paiement (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteMoyenPaiementCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Moyen de paiement {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Moyen de paiement supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
