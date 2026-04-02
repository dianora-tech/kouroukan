using System.Security.Claims;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kouroukan.Api.Gateway.Controllers;

/// <summary>
/// Controleur des forfaits cote utilisateur.
/// Gere le statut d'abonnement, la souscription, la resiliation et le quota d'eleves.
/// Accessible a tout utilisateur authentifie.
/// </summary>
[ApiController]
[Route("api/forfait")]
[Authorize]
public sealed class ForfaitController : ControllerBase
{
    private readonly IForfaitUserService _forfaitUserService;

    public ForfaitController(IForfaitUserService forfaitUserService)
    {
        _forfaitUserService = forfaitUserService;
    }

    /// <summary>
    /// Extrait l'identifiant utilisateur du token JWT.
    /// </summary>
    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? User.FindFirst("sub")?.Value;
        return claim is not null && int.TryParse(claim, out var id) ? id : null;
    }

    /// <summary>
    /// Extrait l'identifiant de l'etablissement du token JWT.
    /// </summary>
    private int? GetCompanyId()
    {
        var claim = User.FindFirst("companyId")?.Value;
        return claim is not null && int.TryParse(claim, out var id) ? id : null;
    }

    /// <summary>
    /// Recupere le statut de l'abonnement actif de l'utilisateur ou de son etablissement.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<ForfaitStatusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(CancellationToken ct)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var result = await _forfaitUserService.GetStatusAsync(companyId, userId, ct);
        return Ok(ApiResponse<ForfaitStatusDto>.Ok(result));
    }

    /// <summary>
    /// Liste les plans forfait disponibles pour un type de cible.
    /// </summary>
    [HttpGet("plans")]
    [ProducesResponseType(typeof(ApiResponse<List<ForfaitPlanDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailablePlans([FromQuery] string type, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(type))
            return BadRequest(ApiResponse<object>.Fail("Le parametre 'type' est requis."));

        var result = await _forfaitUserService.GetAvailablePlansAsync(type, ct);
        return Ok(ApiResponse<List<ForfaitPlanDto>>.Ok(result));
    }

    /// <summary>
    /// Souscrit a un forfait.
    /// </summary>
    [HttpPost("subscribe")]
    [ProducesResponseType(typeof(ApiResponse<AbonnementHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeForfaitRequest request, CancellationToken ct)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        try
        {
            var result = await _forfaitUserService.SubscribeAsync(companyId, userId, request, ct);
            return Ok(ApiResponse<AbonnementHistoryDto>.Ok(result, "Abonnement souscrit avec succes."));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.Fail(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Resilie un abonnement.
    /// </summary>
    [HttpPost("cancel")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel([FromBody] CancelForfaitRequest request, CancellationToken ct)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        try
        {
            await _forfaitUserService.CancelAsync(request.AbonnementId, companyId, userId, ct);
            return Ok(ApiResponse<object>.Ok(null!, "Abonnement resilie avec succes."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Recupere l'historique des abonnements.
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ApiResponse<List<AbonnementHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(CancellationToken ct)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var result = await _forfaitUserService.GetHistoryAsync(companyId, userId, ct);
        return Ok(ApiResponse<List<AbonnementHistoryDto>>.Ok(result));
    }

    /// <summary>
    /// Verifie le quota d'eleves de l'etablissement.
    /// </summary>
    [HttpGet("quota")]
    [ProducesResponseType(typeof(ApiResponse<QuotaCheckResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckStudentQuota(CancellationToken ct)
    {
        var companyId = GetCompanyId();
        if (!companyId.HasValue)
            return BadRequest(ApiResponse<object>.Fail("Aucun etablissement associe."));

        var result = await _forfaitUserService.CheckStudentQuotaAsync(companyId.Value, ct);
        return Ok(ApiResponse<QuotaCheckResult>.Ok(result));
    }
}
