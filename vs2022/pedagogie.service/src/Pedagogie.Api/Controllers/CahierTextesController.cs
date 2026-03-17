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
[Route("api/pedagogie/cahiers-textes")]
[Authorize]
public sealed class CahierTextesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CahierTextesController> _logger;

    public CahierTextesController(IMediator mediator, ILogger<CahierTextesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere tous les cahiers de textes avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<CahierTextes>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET cahiers-textes page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedCahiersTextesQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<CahierTextes>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un cahier de textes par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<CahierTextes>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetCahierTextesByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<CahierTextes>.Fail($"Cahier de textes {id} introuvable."));

        var response = ApiResponse<CahierTextes>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau cahier de textes.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:pedagogie:create")]
    public async Task<ActionResult<ApiResponse<CahierTextes>>> Create(
        [FromBody] CreateCahierTextesCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<CahierTextes>.Ok(created, "Cahier de textes cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un cahier de textes existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateCahierTextesCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Cahier de textes {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Cahier de textes mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un cahier de textes (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteCahierTextesCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Cahier de textes {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Cahier de textes supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
