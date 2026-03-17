using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support.Api.Models;
using Support.Application.Commands;
using Support.Application.Queries;
using Support.Domain.Entities;

namespace Support.Api.Controllers;

/// <summary>
/// Controleur pour la gestion des articles d'aide.
/// </summary>
[ApiController]
[Route("api/support/articles-aide")]
[Authorize]
public sealed class ArticleAideController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ArticleAideController> _logger;

    public ArticleAideController(IMediator mediator, ILogger<ArticleAideController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Liste paginee des articles d'aide.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<PagedResult<ArticleAide>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? typeId = null,
        [FromQuery] string? orderBy = null,
        CancellationToken ct = default)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("GET articles-aide page={Page} [CorrelationId: {CorrelationId}]", page, correlationId);

        var result = await _mediator.Send(new GetPagedArticlesAideQuery(page, pageSize, search, typeId, orderBy), ct);
        var response = ApiResponse<PagedResult<ArticleAide>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere un article d'aide par son identifiant.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<ArticleAide>>> GetById(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var entity = await _mediator.Send(new GetArticleAideByIdQuery(id), ct);
        if (entity is null)
            return NotFound(ApiResponse<ArticleAide>.Fail($"Article d'aide {id} introuvable."));

        var response = ApiResponse<ArticleAide>.Ok(entity);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere les types d'articles d'aide.
    /// </summary>
    [HttpGet("types")]
    [Authorize(Policy = "RequirePermission:support:read")]
    public ActionResult<ApiResponse<string[]>> GetTypes()
    {
        var types = new[]
        {
            "Guide de demarrage", "Tutoriel", "FAQ", "Depannage", "Bonnes pratiques"
        };
        return Ok(ApiResponse<string[]>.Ok(types));
    }

    /// <summary>
    /// Recherche full-text dans les articles d'aide.
    /// </summary>
    [HttpGet("search")]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ArticleAide>>>> Search(
        [FromQuery] string q, [FromQuery] int limit = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(ApiResponse<IReadOnlyList<ArticleAide>>.Fail("Le parametre de recherche 'q' est obligatoire."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new RechercherArticlesAideQuery(q, limit), ct);
        var response = ApiResponse<IReadOnlyList<ArticleAide>>.Ok(result);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Cree un nouvel article d'aide.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:support:create")]
    public async Task<ActionResult<ApiResponse<ArticleAide>>> Create(
        [FromBody] CreateArticleAideCommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<ArticleAide>.Ok(created, "Article d'aide cree avec succes.");
        response.CorrelationId = correlationId;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>
    /// Met a jour un article d'aide existant.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequirePermission:support:update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id, [FromBody] UpdateArticleAideCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<bool>.Fail("L'identifiant de la route ne correspond pas au corps."));

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(command, ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Article d'aide {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Article d'aide mis a jour.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Supprime un article d'aide (soft delete).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:support:delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new DeleteArticleAideCommand(id), ct);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Article d'aide {id} introuvable."));

        var response = ApiResponse<bool>.Ok(true, "Article d'aide supprime.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Marque un article comme utile.
    /// </summary>
    [HttpPost("{id:int}/utile")]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<bool>>> MarquerUtile(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var result = await _mediator.Send(new MarquerArticleUtileCommand(id), ct);
        var response = ApiResponse<bool>.Ok(result, "Article marque comme utile.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}
