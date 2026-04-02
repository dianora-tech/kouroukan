using Finances.Api.Models;
using Finances.Application.Commands;
using Finances.Application.Queries;
using Finances.Domain.Entities;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finances.Api.Controllers;

[ApiController]
[Route("api/finances/journal")]
[Authorize]
public sealed class JournalFinancierController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<JournalFinancierController> _logger;

    public JournalFinancierController(IMediator mediator, ILogger<JournalFinancierController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>Recupere les entrees du journal financier avec pagination et filtres.</summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:finances:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<JournalFinancier>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] int? companyId = null, [FromQuery] string? type = null,
        [FromQuery] string? categorie = null,
        [FromQuery] DateTime? dateDebut = null, [FromQuery] DateTime? dateFin = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET journal page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedJournalFinancierQuery(page, pageSize, companyId, type, categorie, dateDebut, dateFin, orderBy), ct);
        var response = ApiResponse<PagedResult<JournalFinancier>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>Cree une nouvelle entree dans le journal financier.</summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:finances:create")]
    public async Task<ActionResult<ApiResponse<JournalFinancier>>> Create(
        [FromBody] CreateJournalFinancierCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<JournalFinancier>.Ok(created, "Entree journal financier creee avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(null, new { id = created.Id }, response);
    }
}
