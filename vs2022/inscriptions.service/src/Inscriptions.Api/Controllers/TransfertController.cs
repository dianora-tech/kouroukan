using GnDapper.Models;
using Inscriptions.Api.Models;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inscriptions.Api.Controllers;

[ApiController]
[Route("api/inscriptions/transferts")]
[Authorize]
public sealed class TransfertController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransfertController> _logger;

    public TransfertController(IMediator mediator, ILogger<TransfertController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere les transferts avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Transfert>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET transferts page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedTransfertsQuery(page, pageSize, search, orderBy), ct);
        var response = ApiResponse<PagedResult<Transfert>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Initie un nouveau transfert.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:inscriptions:create")]
    public async Task<ActionResult<ApiResponse<Transfert>>> Create(
        [FromBody] CreateTransfertCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Transfert>.Ok(created, "Transfert initie avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(null, new { id = created.Id }, response);
    }

    /// <summary>Accepte un transfert.</summary>
    [HttpPut("{id:int}/accept")]
    [Authorize(Policy = "RequirePermission:inscriptions:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Accept(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new AcceptTransfertCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Transfert {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Transfert accepte.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Refuse un transfert.</summary>
    [HttpPut("{id:int}/reject")]
    [Authorize(Policy = "RequirePermission:inscriptions:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Reject(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new RejectTransfertCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Transfert {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Transfert refuse.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Complete un transfert.</summary>
    [HttpPut("{id:int}/complete")]
    [Authorize(Policy = "RequirePermission:inscriptions:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Complete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new CompleteTransfertCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Transfert {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Transfert complete.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
