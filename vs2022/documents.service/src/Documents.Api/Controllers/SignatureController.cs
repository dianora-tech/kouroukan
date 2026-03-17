using GnDapper.Models;
using Documents.Api.Models;
using Documents.Application.Commands;
using Documents.Application.Queries;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Documents.Api.Controllers;

[ApiController]
[Route("api/documents/signatures")]
[Authorize]
public sealed class SignatureController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISignatureRepository _signatureRepository;
    private readonly ILogger<SignatureController> _logger;

    public SignatureController(
        IMediator mediator,
        ISignatureRepository signatureRepository,
        ILogger<SignatureController> logger)
    {
        _mediator = mediator;
        _signatureRepository = signatureRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de signatures.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:documents:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _signatureRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere toutes les signatures avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:documents:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<Signature>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET signatures page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedSignaturesQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<Signature>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere une signature par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:documents:read")]
    public async Task<ActionResult<ApiResponse<Signature>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetSignatureByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<Signature>.Fail($"Signature {id} introuvable."));

        var response = ApiResponse<Signature>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle signature.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:documents:create")]
    public async Task<ActionResult<ApiResponse<Signature>>> Create(
        [FromBody] CreateSignatureCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<Signature>.Ok(created, "Signature creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour une signature existante.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:documents:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateSignatureCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Signature {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Signature mise a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime une signature (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:documents:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteSignatureCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Signature {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Signature supprimee.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
