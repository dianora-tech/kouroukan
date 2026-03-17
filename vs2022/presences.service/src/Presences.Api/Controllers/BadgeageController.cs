using GnDapper.Models;
using Presences.Api.Models;
using Presences.Application.Commands;
using Presences.Application.Queries;
using Presences.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BadgeageEntity = Presences.Domain.Entities.Badgeage;

namespace Presences.Api.Controllers;

[ApiController]
[Route("api/presences/badgeages")]
[Authorize]
public sealed class BadgeageController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IBadgeageRepository _badgeageRepository;
    private readonly ILogger<BadgeageController> _logger;

    public BadgeageController(
        IMediator mediator,
        IBadgeageRepository badgeageRepository,
        ILogger<BadgeageController> logger)
    {
        _mediator = mediator;
        _badgeageRepository = badgeageRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de badgeages.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:presences:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _badgeageRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere tous les badgeages avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:presences:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<BadgeageEntity>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET badgeages page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedBadgeagesQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<BadgeageEntity>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un badgeage par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:presences:read")]
    public async Task<ActionResult<ApiResponse<BadgeageEntity>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetBadgeageByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<BadgeageEntity>.Fail($"Badgeage {id} introuvable."));

        var response = ApiResponse<BadgeageEntity>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau badgeage.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:presences:create")]
    public async Task<ActionResult<ApiResponse<BadgeageEntity>>> Create(
        [FromBody] CreateBadgeageCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<BadgeageEntity>.Ok(created, "Badgeage cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un badgeage existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:presences:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateBadgeageCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Badgeage {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Badgeage mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un badgeage (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:presences:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteBadgeageCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Badgeage {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Badgeage supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
