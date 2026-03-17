using GnDapper.Models;
using Bde.Api.Models;
using Bde.Application.Commands;
using Bde.Application.Queries;
using Bde.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssociationEntity = Bde.Domain.Entities.Association;

namespace Bde.Api.Controllers;

[ApiController]
[Route("api/bde/associations")]
[Authorize]
public sealed class AssociationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAssociationRepository _associationRepository;
    private readonly ILogger<AssociationController> _logger;

    public AssociationController(
        IMediator mediator,
        IAssociationRepository associationRepository,
        ILogger<AssociationController> logger)
    {
        _mediator = mediator;
        _associationRepository = associationRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types d'associations.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _associationRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere toutes les associations avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<AssociationEntity>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET associations page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedAssociationsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<AssociationEntity>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une association par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:read")]
    public async Task<ActionResult<ApiResponse<AssociationEntity>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetAssociationByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<AssociationEntity>.Fail($"Association {id} introuvable."));

        var response = ApiResponse<AssociationEntity>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle association.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:bde:create")]
    public async Task<ActionResult<ApiResponse<AssociationEntity>>> Create(
        [FromBody] CreateAssociationCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<AssociationEntity>.Ok(created, "Association creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une association existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateAssociationCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Association {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Association mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une association (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:bde:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteAssociationCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Association {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Association supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
