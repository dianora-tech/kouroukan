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
[Route("api/communication/messages")]
[Authorize]
public sealed class MessageController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<MessageController> _logger;

    public MessageController(
        IMediator mediator,
        IMessageRepository messageRepository,
        ILogger<MessageController> logger)
    {
        _mediator = mediator;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de messages.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:communication:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _messageRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere tous les messages avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:communication:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Message>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET messages page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedMessagesQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<Message>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un message par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:communication:read")]
    public async Task<ActionResult<ApiResponse<Message>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetMessageByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Message>.Fail($"Message {id} introuvable."));

        var response = ApiResponse<Message>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau message.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:communication:create")]
    public async Task<ActionResult<ApiResponse<Message>>> Create(
        [FromBody] CreateMessageCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Message>.Ok(created, "Message cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un message existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:communication:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateMessageCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Message {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Message mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un message (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:communication:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteMessageCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Message {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Message supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
