using GnDapper.Models;
using Bde.Api.Models;
using Bde.Application.Commands;
using Bde.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MembreBdeEntity = Bde.Domain.Entities.MembreBde;

namespace Bde.Api.Controllers;

[ApiController]
[Route("api/bde/membres-bde")]
[Authorize]
public sealed class MembreBdeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MembreBdeController> _logger;

    public MembreBdeController(
        IMediator mediator,
        ILogger<MembreBdeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere tous les membres BDE avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<MembreBdeEntity>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET membres-bde page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedMembresBdeQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<MembreBdeEntity>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un membre BDE par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<MembreBdeEntity>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetMembreBdeByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<MembreBdeEntity>.Fail($"Membre BDE {id} introuvable."));

        var response = ApiResponse<MembreBdeEntity>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau membre BDE.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:bde:create")]
    public async Task<ActionResult<ApiResponse<MembreBdeEntity>>> Create(
        [FromBody] CreateMembreBdeCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<MembreBdeEntity>.Ok(created, "Membre BDE cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un membre BDE existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateMembreBdeCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Membre BDE {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Membre BDE mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un membre BDE (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteMembreBdeCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Membre BDE {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Membre BDE supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
