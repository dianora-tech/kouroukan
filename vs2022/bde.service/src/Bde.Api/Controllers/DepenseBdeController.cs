using GnDapper.Models;
using Bde.Api.Models;
using Bde.Application.Commands;
using Bde.Application.Queries;
using Bde.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DepenseBdeEntity = Bde.Domain.Entities.DepenseBde;

namespace Bde.Api.Controllers;

[ApiController]
[Route("api/bde/depenses-bde")]
[Authorize]
public sealed class DepenseBdeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDepenseBdeRepository _depenseBdeRepository;
    private readonly ILogger<DepenseBdeController> _logger;

    public DepenseBdeController(
        IMediator mediator,
        IDepenseBdeRepository depenseBdeRepository,
        ILogger<DepenseBdeController> logger)
    {
        _mediator = mediator;
        _depenseBdeRepository = depenseBdeRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de depenses BDE.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _depenseBdeRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere toutes les depenses BDE avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<DepenseBdeEntity>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET depenses-bde page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedDepensesBdeQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<DepenseBdeEntity>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une depense BDE par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<DepenseBdeEntity>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetDepenseBdeByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<DepenseBdeEntity>.Fail($"Depense BDE {id} introuvable."));

        var response = ApiResponse<DepenseBdeEntity>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle depense BDE.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:bde:create")]
    public async Task<ActionResult<ApiResponse<DepenseBdeEntity>>> Create(
        [FromBody] CreateDepenseBdeCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<DepenseBdeEntity>.Ok(created, "Depense BDE creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une depense BDE existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateDepenseBdeCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Depense BDE {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Depense BDE mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une depense BDE (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteDepenseBdeCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Depense BDE {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Depense BDE supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
