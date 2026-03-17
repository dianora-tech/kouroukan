using GnDapper.Models;
using Inscriptions.Api.Models;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InscriptionEntity = Inscriptions.Domain.Entities.Inscription;

namespace Inscriptions.Api.Controllers;

[ApiController]
[Route("api/inscriptions/inscriptions")]
[Authorize]
public sealed class InscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IInscriptionRepository _inscriptionRepository;
    private readonly ILogger<InscriptionController> _logger;

    public InscriptionController(
        IMediator mediator,
        IInscriptionRepository inscriptionRepository,
        ILogger<InscriptionController> logger)
    {
        _mediator = mediator;
        _inscriptionRepository = inscriptionRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types d'inscriptions.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _inscriptionRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere toutes les inscriptions avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<InscriptionEntity>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET inscriptions page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedInscriptionsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<InscriptionEntity>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une inscription par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<InscriptionEntity>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetInscriptionByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<InscriptionEntity>.Fail($"Inscription {id} introuvable."));

        var response = ApiResponse<InscriptionEntity>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle inscription.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:inscriptions:create")]
    public async Task<ActionResult<ApiResponse<InscriptionEntity>>> Create(
        [FromBody] CreateInscriptionCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<InscriptionEntity>.Ok(created, "Inscription creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une inscription existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateInscriptionCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Inscription {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Inscription mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une inscription (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteInscriptionCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Inscription {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Inscription supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
