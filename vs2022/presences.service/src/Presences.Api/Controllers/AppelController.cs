using GnDapper.Models;
using Presences.Api.Models;
using Presences.Application.Commands;
using Presences.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppelEntity = Presences.Domain.Entities.Appel;

namespace Presences.Api.Controllers;

[ApiController]
[Route("api/presences/appels")]
[Authorize]
public sealed class AppelController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AppelController> _logger;

    public AppelController(
        IMediator mediator,
        ILogger<AppelController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere tous les appels avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:presences:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<AppelEntity>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET appels page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedAppelsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<AppelEntity>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un appel par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:presences:read")]
    public async Task<ActionResult<ApiResponse<AppelEntity>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetAppelByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<AppelEntity>.Fail($"Appel {id} introuvable."));

        var response = ApiResponse<AppelEntity>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouvel appel.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:presences:create")]
    public async Task<ActionResult<ApiResponse<AppelEntity>>> Create(
        [FromBody] CreateAppelCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<AppelEntity>.Ok(created, "Appel cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un appel existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:presences:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateAppelCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Appel {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Appel mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un appel (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:presences:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteAppelCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Appel {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Appel supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
