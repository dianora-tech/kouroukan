using GnDapper.Models;
using Personnel.Api.Models;
using Personnel.Application.Commands;
using Personnel.Application.Queries;
using Personnel.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Personnel.Api.Controllers;

[ApiController]
[Route("api/personnel/enseignants")]
[Authorize]
public sealed class EnseignantController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EnseignantController> _logger;

    public EnseignantController(IMediator mediator, ILogger<EnseignantController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere tous les enseignants avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:personnel:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Enseignant>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] int? typeId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET enseignants page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedEnseignantsQuery(page, pageSize, search, typeId, orderBy), ct);
        var response = ApiResponse<PagedResult<Enseignant>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un enseignant par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:personnel:read")]
    public async Task<ActionResult<ApiResponse<Enseignant>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetEnseignantByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Enseignant>.Fail($"Enseignant {id} introuvable."));

        var response = ApiResponse<Enseignant>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouvel enseignant.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:personnel:create")]
    public async Task<ActionResult<ApiResponse<Enseignant>>> Create(
        [FromBody] CreateEnseignantCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Enseignant>.Ok(created, "Enseignant cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un enseignant existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:personnel:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateEnseignantCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Enseignant {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Enseignant mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un enseignant (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:personnel:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteEnseignantCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Enseignant {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Enseignant supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
