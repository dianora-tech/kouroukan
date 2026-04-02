using GnDapper.Models;
using Pedagogie.Api.Models;
using Pedagogie.Application.Commands;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pedagogie.Api.Controllers;

[ApiController]
[Route("api/pedagogie/competences-enseignant")]
[Authorize]
public sealed class CompetenceEnseignantController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CompetenceEnseignantController> _logger;

    public CompetenceEnseignantController(IMediator mediator, ILogger<CompetenceEnseignantController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere les competences enseignant avec pagination et filtres.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<CompetenceEnseignant>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] int? userId = null, [FromQuery] string? cycleEtude = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET competences-enseignant page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedCompetencesEnseignantQuery(page, pageSize, userId, cycleEtude, orderBy), ct);
        var response = ApiResponse<PagedResult<CompetenceEnseignant>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle competence enseignant.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:pedagogie:create")]
    public async Task<ActionResult<ApiResponse<CompetenceEnseignant>>> Create(
        [FromBody] CreateCompetenceEnseignantCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<CompetenceEnseignant>.Ok(created, "Competence enseignant creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(null, new { id = created.Id }, response);
    }

    /// <summary>Supprime une competence enseignant.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteCompetenceEnseignantCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Competence enseignant {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Competence enseignant supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
