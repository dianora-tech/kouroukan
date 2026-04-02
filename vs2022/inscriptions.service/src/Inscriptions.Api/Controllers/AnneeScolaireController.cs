using GnDapper.Models;
using Inscriptions.Api.Models;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inscriptions.Api.Controllers;

[ApiController]
[Route("api/inscriptions/annees-scolaires")]
[Authorize]
public sealed class AnneeScolaireController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AnneeScolaireController> _logger;

    public AnneeScolaireController(IMediator mediator, ILogger<AnneeScolaireController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere toutes les annees scolaires avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:annees-scolaires:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<AnneeScolaire>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET annees-scolaires page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedAnneeScolairesQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<AnneeScolaire>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une annee scolaire par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:annees-scolaires:read")]
    public async Task<ActionResult<ApiResponse<AnneeScolaire>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetAnneeScolaireByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<AnneeScolaire>.Fail($"Annee scolaire {id} introuvable."));

        var response = ApiResponse<AnneeScolaire>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle annee scolaire.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:annees-scolaires:create")]
    public async Task<ActionResult<ApiResponse<AnneeScolaire>>> Create(
        [FromBody] CreateAnneeScolaireCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<AnneeScolaire>.Ok(created, "Annee scolaire creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une annee scolaire existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:annees-scolaires:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateAnneeScolaireCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Annee scolaire {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Annee scolaire mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une annee scolaire (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:annees-scolaires:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteAnneeScolaireCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Annee scolaire {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Annee scolaire supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
