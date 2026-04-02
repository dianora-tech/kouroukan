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
[Route("api/pedagogie/affectations-enseignant")]
[Authorize]
public sealed class AffectationEnseignantController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AffectationEnseignantController> _logger;

    public AffectationEnseignantController(IMediator mediator, ILogger<AffectationEnseignantController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere les affectations enseignant avec pagination et filtres.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:pedagogie:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<AffectationEnseignant>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] int? liaisonId = null, [FromQuery] int? classeId = null,
        [FromQuery] int? matiereId = null, [FromQuery] int? anneeScolaireId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET affectations-enseignant page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedAffectationsEnseignantQuery(page, pageSize, liaisonId, classeId, matiereId, anneeScolaireId, orderBy), ct);
        var response = ApiResponse<PagedResult<AffectationEnseignant>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle affectation enseignant.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:pedagogie:create")]
    public async Task<ActionResult<ApiResponse<AffectationEnseignant>>> Create(
        [FromBody] CreateAffectationEnseignantCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<AffectationEnseignant>.Ok(created, "Affectation enseignant creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(null, new { id = created.Id }, response);
    }

    /// <summary>Met a jour une affectation enseignant (activation/desactivation).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateAffectationEnseignantCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Affectation enseignant {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Affectation enseignant mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une affectation enseignant (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:pedagogie:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteAffectationEnseignantCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Affectation enseignant {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Affectation enseignant supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
