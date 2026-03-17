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
[Route("api/pedagogie/classes")]
[Authorize]
public sealed class ClasseController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ClasseController> _logger;

    public ClasseController(IMediator mediator, ILogger<ClasseController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere toutes les classes avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Classe>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET classes page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedClassesQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<Classe>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une classe par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<Classe>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetClasseByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Classe>.Fail($"Classe {id} introuvable."));

        var response = ApiResponse<Classe>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle classe.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:pedagogie:create")]
    public async Task<ActionResult<ApiResponse<Classe>>> Create(
        [FromBody] CreateClasseCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Classe>.Ok(created, "Classe creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une classe existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateClasseCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Classe {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Classe mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une classe (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteClasseCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Classe {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Classe supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
