using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support.Api.Models;
using Support.Application.Commands;
using Support.Application.Queries;
using Support.Domain.Entities;

namespace Support.Api.Controllers;

/// <summary>
/// Controleur pour la gestion des suggestions d'amelioration.
/// </summary>
[ApiController]
[Route("api/support/suggestions")]
[Authorize]
public sealed class SuggestionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SuggestionController> _logger;

    public SuggestionController(IMediator mediator, ILogger<SuggestionController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Liste paginee des suggestions.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Suggestion>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? typeId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET suggestions page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedSuggestionsQuery(page, pageSize, search, typeId, orderBy), ct);
        var response = ApiResponse<PagedResult<Suggestion>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere une suggestion par son identifiant.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<Suggestion>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetSuggestionByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Suggestion>.Fail($"Suggestion {id} introuvable."));

        var response = ApiResponse<Suggestion>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere les types de suggestions.
    /// </summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:support:read")]
    public ActionResult<ApiResponse<string[]>> GetTypes()
    {
        var types = new[]
        {
            "Nouvelle fonctionnalite", "Amelioration existante", "Interface utilisateur",
            "Performance", "Contenu pedagogique"
        };
        return Ok(ApiResponse<string[]>.Ok(types));
    }

    /// <summary>
    /// Cree une nouvelle suggestion.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:support:create")]
    public async Task<ActionResult<ApiResponse<Suggestion>>> Create(
        [FromBody] CreateSuggestionCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Suggestion>.Ok(created, "Suggestion creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>
    /// Met a jour une suggestion existante.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:support:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateSuggestionCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Suggestion {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Suggestion mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Supprime une suggestion (soft delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:support:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteSuggestionCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Suggestion {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Suggestion supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Vote pour une suggestion.
    /// </summary>
    [HttpPost("{id:int}/vote")]
    [Authorize(Policy = "RequirePermission:support:create")]
    public async Task<ActionResult<ApiResponse<bool>>> Voter(
        int id, [FromBody] VoterSuggestionCommand command, CancellationToken ct)
    {
        if (id != command.SuggestionId)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la suggestion ne correspond pas."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        var response = ApiResponse<bool>.Ok(result, "Vote enregistre.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Retire le vote pour une suggestion.
    /// </summary>
    [HttpDelete("{id:int}/vote")]
    [Authorize(Policy = "RequirePermission:support:create")]
    public async Task<ActionResult<ApiResponse<bool>>> RetirerVote(
        int id, [FromQuery] int votantId, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new RetirerVoteSuggestionCommand(id, votantId), ct);
        var response = ApiResponse<bool>.Ok(result, "Vote retire.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
