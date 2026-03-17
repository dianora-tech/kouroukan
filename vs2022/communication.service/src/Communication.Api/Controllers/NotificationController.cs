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
[Route("api/communication/notifications")]
[Authorize]
public sealed class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        IMediator mediator,
        INotificationRepository notificationRepository,
        ILogger<NotificationController> logger)
    {
        _mediator = mediator;
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de notifications.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:communication:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _notificationRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere toutes les notifications avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:communication:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Notification>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET notifications page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedNotificationsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<Notification>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une notification par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:communication:read")]
    public async Task<ActionResult<ApiResponse<Notification>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetNotificationByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Notification>.Fail($"Notification {id} introuvable."));

        var response = ApiResponse<Notification>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle notification.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:communication:create")]
    public async Task<ActionResult<ApiResponse<Notification>>> Create(
        [FromBody] CreateNotificationCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Notification>.Ok(created, "Notification creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une notification existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:communication:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateNotificationCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Notification {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Notification mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une notification (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:communication:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteNotificationCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Notification {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Notification supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
