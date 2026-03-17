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
/// Controleur pour la gestion des factures.
/// </summary>
[ApiController]
[Route("api/finances/factures")]
[Authorize]
public sealed class FactureController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FactureController> _logger;

    public FactureController(IMediator mediator, ILogger<FactureController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Recupere la liste paginee des factures.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Facture>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? typeId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET factures page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedFacturesQuery(page, pageSize, search, typeId, orderBy), ct);
        var response = ApiResponse<PagedResult<Facture>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere une facture par son identifiant.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<Facture>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetFactureByIdQuery(id), ct);

        if (entity is null)
            return NotFound(ApiResponse<Facture>.Fail($"Facture {id} introuvable."));

        var response = ApiResponse<Facture>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere les types de factures.
    /// </summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<Facture>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new GetAllFacturesQuery(), ct);
        var response = ApiResponse<IReadOnlyList<Facture>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Cree une nouvelle facture.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:finances:create")]
    public async Task<ActionResult<ApiResponse<Facture>>> Create(
        [FromBody] CreateFactureCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("POST facture [CorrelationId: {CorrelationId}]", correlationId);

        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Facture>.Ok(created, "Facture creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>
    /// Met a jour une facture existante.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateFactureCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);

        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Facture {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Facture mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Supprime une facture (soft delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteFactureCommand(id), ct);

        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Facture {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Facture supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
