using Finances.Api.Models;
using Finances.Application.Commands;
using Finances.Application.Queries;
using Finances.Domain.Entities;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finances.Api.Controllers;

/// <summary>
/// Controleur pour la gestion des remunerations des enseignants.
/// </summary>
[ApiController]
[Route("api/finances/remunerations-enseignants")]
[Authorize]
public sealed class RemunerationEnseignantController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RemunerationEnseignantController> _logger;

    public RemunerationEnseignantController(IMediator mediator, ILogger<RemunerationEnseignantController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Recupere la liste paginee des remunerations enseignants.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<RemunerationEnseignant>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET remunerations-enseignants page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedRemunerationsEnseignantsQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<RemunerationEnseignant>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere une remuneration par son identifiant.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<RemunerationEnseignant>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetRemunerationEnseignantByIdQuery(id), ct);

        if (entity is null)
            return NotFound(ApiResponse<RemunerationEnseignant>.Fail($"Remuneration {id} introuvable."));

        var response = ApiResponse<RemunerationEnseignant>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Cree une nouvelle remuneration enseignant.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:finances:create")]
    public async Task<ActionResult<ApiResponse<RemunerationEnseignant>>> Create(
        [FromBody] CreateRemunerationEnseignantCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("POST remuneration-enseignant [CorrelationId: {CorrelationId}]", correlationId);

        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<RemunerationEnseignant>.Ok(created, "Remuneration creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>
    /// Met a jour une remuneration existante.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateRemunerationEnseignantCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);

        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Remuneration {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Remuneration mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Supprime une remuneration (soft delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:finances:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteRemunerationEnseignantCommand(id), ct);

        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Remuneration {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Remuneration supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
