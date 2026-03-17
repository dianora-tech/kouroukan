using GnDapper.Models;
using Pedagogie.Api.Models;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pedagogie.Api.Controllers;

[ApiController]
[Route("api/pedagogie/seances")]
[Authorize]
public sealed class SeanceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SeanceController> _logger;

    public SeanceController(IMediator mediator, ILogger<SeanceController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere toutes les seances avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Seance>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET seances page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedSeancesQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<Seance>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une seance par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<Seance>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetSeanceByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Seance>.Fail($"Seance {id} introuvable."));

        var response = ApiResponse<Seance>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle seance.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:pedagogie:create")]
    public async Task<ActionResult<ApiResponse<Seance>>> Create(
        [FromBody] CreateSeanceCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Seance>.Ok(created, "Seance creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une seance existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateSeanceCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Seance {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Seance mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une seance (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteSeanceCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Seance {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Seance supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
