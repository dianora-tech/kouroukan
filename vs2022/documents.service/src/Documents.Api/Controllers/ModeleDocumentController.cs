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
[Route("api/documents/modeles-documents")]
[Authorize]
public sealed class ModeleDocumentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IModeleDocumentRepository _modeleDocumentRepository;
    private readonly ILogger<ModeleDocumentController> _logger;

    public ModeleDocumentController(
        IMediator mediator,
        IModeleDocumentRepository modeleDocumentRepository,
        ILogger<ModeleDocumentController> logger)
    {
        _mediator = mediator;
        _modeleDocumentRepository = modeleDocumentRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de modeles de documents.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:documents:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _modeleDocumentRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere tous les modeles de documents avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:documents:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<ModeleDocument>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET modeles-documents page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedModeleDocumentsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<ModeleDocument>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un modele de document par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:documents:read")]
    public async Task<ActionResult<ApiResponse<ModeleDocument>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetModeleDocumentByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<ModeleDocument>.Fail($"Modele de document {id} introuvable."));

        var response = ApiResponse<ModeleDocument>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau modele de document.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:documents:create")]
    public async Task<ActionResult<ApiResponse<ModeleDocument>>> Create(
        [FromBody] CreateModeleDocumentCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<ModeleDocument>.Ok(created, "Modele de document cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un modele de document existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:documents:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateModeleDocumentCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Modele de document {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Modele de document mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un modele de document (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:documents:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteModeleDocumentCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Modele de document {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Modele de document supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
