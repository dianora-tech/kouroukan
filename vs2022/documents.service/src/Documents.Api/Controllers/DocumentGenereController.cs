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
[Route("api/documents/documents-generes")]
[Authorize]
public sealed class DocumentGenereController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDocumentGenereRepository _documentGenereRepository;
    private readonly ILogger<DocumentGenereController> _logger;

    public DocumentGenereController(
        IMediator mediator,
        IDocumentGenereRepository documentGenereRepository,
        ILogger<DocumentGenereController> logger)
    {
        _mediator = mediator;
        _documentGenereRepository = documentGenereRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de documents generes.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:documents:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _documentGenereRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere tous les documents generes avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:documents:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<DocumentGenere>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET documents-generes page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedDocumentGeneresQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<DocumentGenere>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un document genere par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:documents:read")]
    public async Task<ActionResult<ApiResponse<DocumentGenere>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetDocumentGenereByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<DocumentGenere>.Fail($"Document genere {id} introuvable."));

        var response = ApiResponse<DocumentGenere>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau document genere.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:documents:create")]
    public async Task<ActionResult<ApiResponse<DocumentGenere>>> Create(
        [FromBody] CreateDocumentGenereCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<DocumentGenere>.Ok(created, "Document genere cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un document genere existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:documents:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateDocumentGenereCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Document genere {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Document genere mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un document genere (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:documents:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteDocumentGenereCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Document genere {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Document genere supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
