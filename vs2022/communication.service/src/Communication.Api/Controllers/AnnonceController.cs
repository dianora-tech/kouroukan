using GnDapper.Models;
using Communication.Api.Models;
using Communication.Application.Commands;
using Communication.Application.Queries;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Communication.Api.Controllers;

[ApiController]
[Route("api/communication/annonces")]
[Authorize]
public sealed class AnnonceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAnnonceRepository _annonceRepository;
    private readonly ILogger<AnnonceController> _logger;

    public AnnonceController(
        IMediator mediator,
        IAnnonceRepository annonceRepository,
        ILogger<AnnonceController> logger)
    {
        _mediator = mediator;
        _annonceRepository = annonceRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types d'annonces.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:communication:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _annonceRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere toutes les annonces avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:communication:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Annonce>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET annonces page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedAnnoncesQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<Annonce>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une annonce par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:communication:read")]
    public async Task<ActionResult<ApiResponse<Annonce>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetAnnonceByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Annonce>.Fail($"Annonce {id} introuvable."));

        var response = ApiResponse<Annonce>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle annonce.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:communication:create")]
    public async Task<ActionResult<ApiResponse<Annonce>>> Create(
        [FromBody] CreateAnnonceCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Annonce>.Ok(created, "Annonce creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une annonce existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:communication:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateAnnonceCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Annonce {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Annonce mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une annonce (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:communication:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteAnnonceCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Annonce {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Annonce supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
