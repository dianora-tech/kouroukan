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
/// Controleur pour la gestion des tickets de support.
/// </summary>
[ApiController]
[Route("api/support/tickets")]
[Authorize]
public sealed class TicketController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketController> _logger;

    public TicketController(IMediator mediator, ILogger<TicketController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Liste paginee des tickets.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Ticket>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? typeId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET tickets page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedTicketsQuery(page, pageSize, search, typeId, orderBy), ct);
        var response = ApiResponse<PagedResult<Ticket>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere un ticket par son identifiant.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<Ticket>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetTicketByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Ticket>.Fail($"Ticket {id} introuvable."));

        var response = ApiResponse<Ticket>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Cree un nouveau ticket.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:support:create")]
    public async Task<ActionResult<ApiResponse<Ticket>>> Create(
        [FromBody] CreateTicketCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Ticket>.Ok(created, "Ticket cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>
    /// Met a jour un ticket existant.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:support:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateTicketCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Ticket {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Ticket mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Supprime un ticket (soft delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:support:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteTicketCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Ticket {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Ticket supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere les reponses d'un ticket.
    /// </summary>
    [HttpGet("{id:int}/reponses")]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ReponseTicket>>>> GetReponses(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var reponses = await _mediator.Send(new GetReponsesTicketQuery(id), ct);
        var response = ApiResponse<IReadOnlyList<ReponseTicket>>.Ok(reponses);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Ajoute une reponse a un ticket.
    /// </summary>
    [HttpPost("{id:int}/reponses")]
    [Authorize(Policy = "RequirePermission:support:create")]
    public async Task<ActionResult<ApiResponse<ReponseTicket>>> AddReponse(
        int id, [FromBody] AddReponseTicketCommand command, CancellationToken ct)
    {
        if (id != command.TicketId)
            return BadRequest(ApiResponse<ReponseTicket>.Fail("L'identifiant du ticket ne correspond pas."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<ReponseTicket>.Ok(created, "Reponse ajoutee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Genere une suggestion de reponse IA pour un ticket.
    /// </summary>
    [HttpPost("{id:int}/reponse-ia")]
    [Authorize(Policy = "RequirePermission:support:manage")]
    public async Task<ActionResult<ApiResponse<string>>> GenererReponseIA(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var reponse = await _mediator.Send(new GenererReponseIATicketCommand(id), ct);
        var response = ApiResponse<string>.Ok(reponse, "Suggestion de reponse IA generee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
