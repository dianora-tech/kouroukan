using GnDapper.Models;
using Bde.Api.Models;
using Bde.Application.Commands;
using Bde.Application.Queries;
using Bde.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EvenementEntity = Bde.Domain.Entities.Evenement;

namespace Bde.Api.Controllers;

[ApiController]
[Route("api/bde/evenements")]
[Authorize]
public sealed class EvenementController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IEvenementRepository _evenementRepository;
    private readonly ILogger<EvenementController> _logger;

    public EvenementController(
        IMediator mediator,
        IEvenementRepository evenementRepository,
        ILogger<EvenementController> logger)
    {
        _mediator = mediator;
        _evenementRepository = evenementRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types d'evenements.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _evenementRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere tous les evenements avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<EvenementEntity>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET evenements page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedEvenementsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<EvenementEntity>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un evenement par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<EvenementEntity>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetEvenementByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<EvenementEntity>.Fail($"Evenement {id} introuvable."));

        var response = ApiResponse<EvenementEntity>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouvel evenement.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:bde:create")]
    public async Task<ActionResult<ApiResponse<EvenementEntity>>> Create(
        [FromBody] CreateEvenementCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<EvenementEntity>.Ok(created, "Evenement cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un evenement existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateEvenementCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Evenement {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Evenement mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un evenement (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteEvenementCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Evenement {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Evenement supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
