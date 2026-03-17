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
[Route("api/pedagogie/salles")]
[Authorize]
public sealed class SalleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISalleRepository _salleRepository;
    private readonly ILogger<SalleController> _logger;

    public SalleController(
        IMediator mediator,
        ISalleRepository salleRepository,
        ILogger<SalleController> logger)
    {
        _mediator = mediator;
        _salleRepository = salleRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de salles.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _salleRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere toutes les salles avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Salle>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET salles page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedSallesQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<Salle>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une salle par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<Salle>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetSalleByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Salle>.Fail($"Salle {id} introuvable."));

        var response = ApiResponse<Salle>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle salle.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:pedagogie:create")]
    public async Task<ActionResult<ApiResponse<Salle>>> Create(
        [FromBody] CreateSalleCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Salle>.Ok(created, "Salle creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une salle existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateSalleCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Salle {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Salle mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une salle (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteSalleCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Salle {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Salle supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
