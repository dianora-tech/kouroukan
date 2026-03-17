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
[Route("api/inscriptions/eleves")]
[Authorize]
public sealed class EleveController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EleveController> _logger;

    public EleveController(IMediator mediator, ILogger<EleveController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere tous les eleves avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Eleve>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET eleves page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedElevesQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<Eleve>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un eleve par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<Eleve>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetEleveByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Eleve>.Fail($"Eleve {id} introuvable."));

        var response = ApiResponse<Eleve>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouvel eleve.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:inscriptions:create")]
    public async Task<ActionResult<ApiResponse<Eleve>>> Create(
        [FromBody] CreateEleveCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Eleve>.Ok(created, "Eleve cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un eleve existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateEleveCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Eleve {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Eleve mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un eleve (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteEleveCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Eleve {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Eleve supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
