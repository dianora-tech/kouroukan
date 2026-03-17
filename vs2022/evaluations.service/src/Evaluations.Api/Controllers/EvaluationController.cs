using GnDapper.Models;
using Evaluations.Api.Models;
using Evaluations.Application.Commands;
using Evaluations.Application.Queries;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluations.Api.Controllers;

[ApiController]
[Route("api/evaluations/evaluations")]
[Authorize]
public sealed class EvaluationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ILogger<EvaluationController> _logger;

    public EvaluationController(
        IMediator mediator,
        IEvaluationRepository evaluationRepository,
        ILogger<EvaluationController> logger)
    {
        _mediator = mediator;
        _evaluationRepository = evaluationRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types d'evaluations.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:evaluations:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _evaluationRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere toutes les evaluations avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:evaluations:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Evaluation>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET evaluations page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedEvaluationsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<Evaluation>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une evaluation par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:evaluations:read")]
    public async Task<ActionResult<ApiResponse<Evaluation>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetEvaluationByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Evaluation>.Fail($"Evaluation {id} introuvable."));

        var response = ApiResponse<Evaluation>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle evaluation.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:evaluations:create")]
    public async Task<ActionResult<ApiResponse<Evaluation>>> Create(
        [FromBody] CreateEvaluationCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Evaluation>.Ok(created, "Evaluation creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une evaluation existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:evaluations:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateEvaluationCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Evaluation {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Evaluation mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une evaluation (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:evaluations:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteEvaluationCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Evaluation {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Evaluation supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
