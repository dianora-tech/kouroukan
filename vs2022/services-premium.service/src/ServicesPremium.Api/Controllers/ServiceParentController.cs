using GnDapper.Entities;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesPremium.Api.Models;
using ServicesPremium.Application.Commands;
using ServicesPremium.Application.Queries;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Output;

namespace ServicesPremium.Api.Controllers;

/// <summary>
/// Controleur pour la gestion des services parents (premium).
/// </summary>
[ApiController]
[Route("api/services-premium/services-parents")]
[Authorize]
public sealed class ServiceParentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IServiceParentRepository _serviceParentRepository;
    private readonly ILogger<ServiceParentController> _logger;

    public ServiceParentController(
        IMediator mediator,
        IServiceParentRepository serviceParentRepository,
        ILogger<ServiceParentController> logger)
    {
        _mediator = mediator;
        _serviceParentRepository = serviceParentRepository;
        _logger = logger;
    }

    /// <summary>Obtient les types de services parents.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:services-premium:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET services-parents/types [CorrelationId: {CorrelationId}]", correlationId);

        var types = await _mediator.Send(new GetAllServiceParentsQuery(), ct);
        var typeDtos = await _serviceParentRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(typeDtos);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Obtient tous les services parents avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:services-premium:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<ServiceParent>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET services-parents page={Page} [CorrelationId: {CorrelationId}]",
            page, correlationId);

        var result = await _mediator.Send(
            new GetPagedServiceParentsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<ServiceParent>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Obtient un service parent par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:services-premium:read")]
    public async Task<ActionResult<ApiResponse<ServiceParent>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET services-parents/{Id} [CorrelationId: {CorrelationId}]", id, correlationId);

        var entity = await _mediator.Send(new GetServiceParentByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<ServiceParent>.Fail($"ServiceParent {id} introuvable."));

        var response = ApiResponse<ServiceParent>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau service parent.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:services-premium:create")]
    public async Task<ActionResult<ApiResponse<ServiceParent>>> Create(
        [FromBody] CreateServiceParentCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("POST services-parents — Code: {Code} [CorrelationId: {CorrelationId}]",
            command.Code, correlationId);

        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<ServiceParent>.Ok(created, "Service parent cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un service parent existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:services-premium:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateServiceParentCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant ne correspond pas."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("PUT services-parents/{Id} [CorrelationId: {CorrelationId}]", id, correlationId);

        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"ServiceParent {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Service parent mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un service parent (soft delete).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:services-premium:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("DELETE services-parents/{Id} [CorrelationId: {CorrelationId}]", id, correlationId);

        var result = await _mediator.Send(new DeleteServiceParentCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"ServiceParent {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Service parent supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
