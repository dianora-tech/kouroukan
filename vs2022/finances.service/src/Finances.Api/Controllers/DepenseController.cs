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
/// Controleur pour la gestion des depenses avec workflow de validation.
/// </summary>
[ApiController]
[Route("api/finances/depenses")]
[Authorize]
public sealed class DepenseController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DepenseController> _logger;

    public DepenseController(IMediator mediator, ILogger<DepenseController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Recupere la liste paginee des depenses.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Depense>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? typeId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET depenses page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedDepensesQuery(page, pageSize, search, typeId, orderBy), ct);
        var response = ApiResponse<PagedResult<Depense>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere une depense par son identifiant.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<Depense>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetDepenseByIdQuery(id), ct);

        if (entity is null)
            return NotFound(ApiResponse<Depense>.Fail($"Depense {id} introuvable."));

        var response = ApiResponse<Depense>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere les types de depenses.
    /// </summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<Depense>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new GetAllDepensesQuery(), ct);
        var response = ApiResponse<IReadOnlyList<Depense>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Cree une nouvelle demande de depense.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:finances:create")]
    public async Task<ActionResult<ApiResponse<Depense>>> Create(
        [FromBody] CreateDepenseCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("POST depense [CorrelationId: {CorrelationId}]", correlationId);

        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Depense>.Ok(created, "Depense creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>
    /// Met a jour une depense (changement de statut, validation, etc.).
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateDepenseCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);

        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Depense {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Depense mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Supprime une depense (soft delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteDepenseCommand(id), ct);

        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Depense {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Depense supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
