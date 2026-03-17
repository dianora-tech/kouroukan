using GnDapper.Models;
using Inscriptions.Api.Models;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Output;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inscriptions.Api.Controllers;

[ApiController]
[Route("api/inscriptions/dossiers-admission")]
[Authorize]
public sealed class DossierAdmissionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDossierAdmissionRepository _dossierAdmissionRepository;
    private readonly ILogger<DossierAdmissionController> _logger;

    public DossierAdmissionController(
        IMediator mediator,
        IDossierAdmissionRepository dossierAdmissionRepository,
        ILogger<DossierAdmissionController> logger)
    {
        _mediator = mediator;
        _dossierAdmissionRepository = dossierAdmissionRepository;
        _logger = logger;
    }

    /// <summary>Recupere les types de dossiers d'admission.</summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TypeDto>>>> GetTypes(CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var types = await _dossierAdmissionRepository.GetTypesAsync(ct);
        var response = ApiResponse<IReadOnlyList<TypeDto>>.Ok(types);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere tous les dossiers d'admission avec pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<DossierAdmission>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? orderBy = null,
        [FromQuery] int? typeId = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET dossiers-admission page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedDossierAdmissionsQuery(page, pageSize, search, orderBy, typeId), ct);
        var response = ApiResponse<PagedResult<DossierAdmission>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Recupere un dossier d'admission par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:read")]
    public async Task<ActionResult<ApiResponse<DossierAdmission>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetDossierAdmissionByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<DossierAdmission>.Fail($"Dossier d'admission {id} introuvable."));

        var response = ApiResponse<DossierAdmission>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree un nouveau dossier d'admission.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:inscriptions:create")]
    public async Task<ActionResult<ApiResponse<DossierAdmission>>> Create(
        [FromBody] CreateDossierAdmissionCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<DossierAdmission>.Ok(created, "Dossier d'admission cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>Met a jour un dossier d'admission existant.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateDossierAdmissionCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Dossier d'admission {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Dossier d'admission mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Supprime un dossier d'admission (suppression logique).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:inscriptions:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteDossierAdmissionCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Dossier d'admission {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Dossier d'admission supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
