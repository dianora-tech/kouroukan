using GnDapper.Models;
using Evaluations.Api.Models;
using Evaluations.Application.Commands;
using Evaluations.Application.Queries;
using Evaluations.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluations.Api.Controllers;

[ApiController]
[Route("api/evaluations/notes")]
[Authorize]
public sealed class NoteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NoteController> _logger;

    public NoteController(IMediator mediator, ILogger<NoteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere toutes les notes avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:evaluations:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Note>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET notes page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedNotesQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<Note>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une note par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:evaluations:read")]
    public async Task<ActionResult<ApiResponse<Note>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetNoteByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Note>.Fail($"Note {id} introuvable."));

        var response = ApiResponse<Note>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle note.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:evaluations:create")]
    public async Task<ActionResult<ApiResponse<Note>>> Create(
        [FromBody] CreateNoteCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Note>.Ok(created, "Note creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une note existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:evaluations:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateNoteCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Note {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Note mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une note (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:evaluations:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteNoteCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Note {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Note supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
