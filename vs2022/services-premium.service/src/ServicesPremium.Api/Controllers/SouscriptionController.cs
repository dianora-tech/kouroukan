using GnDapper.Entities;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesPremium.Api.Models;
using ServicesPremium.Application.Commands;
using ServicesPremium.Application.Queries;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Api.Controllers;

/// <summary>
/// Controleur pour la gestion des souscriptions aux services premium.
/// </summary>
[ApiController]
[Route("api/services-premium/souscriptions")]
[Authorize]
public sealed class SouscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SouscriptionController> _logger;

    public SouscriptionController(
        IMediator mediator,
        ILogger<SouscriptionController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Obtient toutes les souscriptions avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:services-premium:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Souscription>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET souscriptions page={Page} [CorrelationId: {CorrelationId}]",
            page, correlationId);

        var result = await _mediator.Send(
            new GetPagedSouscriptionsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<Souscription>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Obtient une souscription par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:services-premium:read")]
    public async Task<ActionResult<ApiResponse<Souscription>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET souscriptions/{Id} [CorrelationId: {CorrelationId}]", id, correlationId);

        var entity = await _mediator.Send(new GetSouscriptionByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Souscription>.Fail($"Souscription {id} introuvable."));

        var response = ApiResponse<Souscription>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle souscription.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:services-premium:create")]
    public async Task<ActionResult<ApiResponse<Souscription>>> Create(
        [FromBody] CreateSouscriptionCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("POST souscriptions — ServiceParent: {ServiceParentId}, Parent: {ParentId} [CorrelationId: {CorrelationId}]",
            command.ServiceParentId, command.ParentId, correlationId);

        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Souscription>.Ok(created, "Souscription creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une souscription existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:services-premium:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateSouscriptionCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant ne correspond pas."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("PUT souscriptions/{Id} [CorrelationId: {CorrelationId}]", id, correlationId);

        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Souscription {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Souscription mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une souscription (soft delete).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:services-premium:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("DELETE souscriptions/{Id} [CorrelationId: {CorrelationId}]", id, correlationId);

        var result = await _mediator.Send(new DeleteSouscriptionCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Souscription {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Souscription supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
