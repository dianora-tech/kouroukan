using Finances.Api.Models;
using Finances.Application.Commands;
using Finances.Application.Queries;
using Finances.Domain.Entities;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finances.Api.Controllers;

/// <summary>
/// Controleur pour la gestion des paiements (Mobile Money + especes).
/// </summary>
[ApiController]
[Route("api/finances/paiements")]
[Authorize]
public sealed class PaiementController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaiementController> _logger;

    public PaiementController(IMediator mediator, ILogger<PaiementController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Recupere la liste paginee des paiements.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Paiement>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? typeId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET paiements page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedPaiementsQuery(page, pageSize, search, typeId, orderBy), ct);
        var response = ApiResponse<PagedResult<Paiement>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere un paiement par son identifiant.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<Paiement>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetPaiementByIdQuery(id), ct);

        if (entity is null)
            return NotFound(ApiResponse<Paiement>.Fail($"Paiement {id} introuvable."));

        var response = ApiResponse<Paiement>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere les types de paiements.
    /// </summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<Paiement>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new GetAllPaiementsQuery(), ct);
        var response = ApiResponse<IReadOnlyList<Paiement>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Enregistre un nouveau paiement.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:finances:create")]
    public async Task<ActionResult<ApiResponse<Paiement>>> Create(
        [FromBody] CreatePaiementCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("POST paiement [CorrelationId: {CorrelationId}]", correlationId);

        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Paiement>.Ok(created, "Paiement enregistre avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>
    /// Met a jour un paiement existant.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdatePaiementCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);

        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Paiement {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Paiement mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Supprime un paiement (soft delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeletePaiementCommand(id), ct);

        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Paiement {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Paiement supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
