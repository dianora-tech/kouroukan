using GnDapper.Models;
using Pedagogie.Api.Models;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pedagogie.Api.Controllers;

[ApiController]
[Route("api/pedagogie/niveaux-classes")]
[Authorize]
public sealed class NiveauClasseController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly INiveauClasseRepository _niveauClasseRepository;
    private readonly ILogger<NiveauClasseController> _logger;

    public NiveauClasseController(
        IMediator mediator,
        INiveauClasseRepository niveauClasseRepository,
        ILogger<NiveauClasseController> logger)
    {
        _mediator = mediator;
        _niveauClasseRepository = niveauClasseRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de niveaux de classes.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _niveauClasseRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere tous les niveaux de classes avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<NiveauClasse>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET niveaux-classes page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedNiveauClassesQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<NiveauClasse>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un niveau de classe par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<NiveauClasse>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetNiveauClasseByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<NiveauClasse>.Fail($"Niveau de classe {id} introuvable."));

        var response = ApiResponse<NiveauClasse>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau niveau de classe.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:pedagogie:create")]
    public async Task<ActionResult<ApiResponse<NiveauClasse>>> Create(
        [FromBody] CreateNiveauClasseCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<NiveauClasse>.Ok(created, "Niveau de classe cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un niveau de classe existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateNiveauClasseCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Niveau de classe {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Niveau de classe mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un niveau de classe (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteNiveauClasseCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Niveau de classe {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Niveau de classe supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
