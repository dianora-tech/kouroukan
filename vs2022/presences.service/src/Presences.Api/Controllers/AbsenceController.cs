using GnDapper.Models;
using Presences.Api.Models;
using Presences.Application.Commands;
using Presences.Application.Queries;
using Presences.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AbsenceEntity = Presences.Domain.Entities.Absence;

namespace Presences.Api.Controllers;

[ApiController]
[Route("api/presences/absences")]
[Authorize]
public sealed class AbsenceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAbsenceRepository _absenceRepository;
    private readonly ILogger<AbsenceController> _logger;

    public AbsenceController(
        IMediator mediator,
        IAbsenceRepository absenceRepository,
        ILogger<AbsenceController> logger)
    {
        _mediator = mediator;
        _absenceRepository = absenceRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types d'absences.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:presences:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _absenceRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere toutes les absences avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:presences:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<AbsenceEntity>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET absences page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedAbsencesQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<AbsenceEntity>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une absence par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:presences:read")]
    public async Task<ActionResult<ApiResponse<AbsenceEntity>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetAbsenceByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<AbsenceEntity>.Fail($"Absence {id} introuvable."));

        var response = ApiResponse<AbsenceEntity>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle absence.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:presences:create")]
    public async Task<ActionResult<ApiResponse<AbsenceEntity>>> Create(
        [FromBody] CreateAbsenceCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<AbsenceEntity>.Ok(created, "Absence creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une absence existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:presences:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateAbsenceCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Absence {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Absence mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une absence (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:presences:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteAbsenceCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Absence {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Absence supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
