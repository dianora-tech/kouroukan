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
[Route("api/evaluations/bulletins")]
[Authorize]
public sealed class BulletinController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BulletinController> _logger;

    public BulletinController(IMediator mediator, ILogger<BulletinController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere tous les bulletins avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:evaluations:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Bulletin>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET bulletins page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedBulletinsQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<Bulletin>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un bulletin par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:evaluations:read")]
    public async Task<ActionResult<ApiResponse<Bulletin>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetBulletinByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Bulletin>.Fail($"Bulletin {id} introuvable."));

        var response = ApiResponse<Bulletin>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau bulletin.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:evaluations:create")]
    public async Task<ActionResult<ApiResponse<Bulletin>>> Create(
        [FromBody] CreateBulletinCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Bulletin>.Ok(created, "Bulletin cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un bulletin existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:evaluations:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateBulletinCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Bulletin {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Bulletin mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un bulletin (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:evaluations:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteBulletinCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Bulletin {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Bulletin supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
