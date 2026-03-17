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
[Route("api/personnel/demandes-conges")]
[Authorize]
public sealed class DemandeCongeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DemandeCongeController> _logger;

    public DemandeCongeController(IMediator mediator, ILogger<DemandeCongeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere toutes les demandes de conge avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:personnel:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<DemandeConge>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] int? typeId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET demandes-conges page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedDemandesCongesQuery(page, pageSize, search, typeId, orderBy), ct);
        var response = ApiResponse<PagedResult<DemandeConge>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une demande de conge par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:personnel:read")]
    public async Task<ActionResult<ApiResponse<DemandeConge>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetDemandeCongeByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<DemandeConge>.Fail($"Demande de conge {id} introuvable."));

        var response = ApiResponse<DemandeConge>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle demande de conge.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:personnel:create")]
    public async Task<ActionResult<ApiResponse<DemandeConge>>> Create(
        [FromBody] CreateDemandeCongeCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<DemandeConge>.Ok(created, "Demande de conge creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une demande de conge existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:personnel:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateDemandeCongeCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Demande de conge {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Demande de conge mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une demande de conge (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:personnel:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteDemandeCongeCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Demande de conge {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Demande de conge supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
