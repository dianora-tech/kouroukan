using System.Security.Claims;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kouroukan.Api.Gateway.Controllers;

/// <summary>
/// Controleur de gestion des liaisons enseignant-etablissement.
/// Permet de creer, accepter, rejeter, terminer et reintegrer des liaisons.
/// </summary>
[ApiController]
[Route("api/auth/liaisons-enseignant")]
[Authorize]
public sealed class LiaisonEnseignantController : ControllerBase
{
    private readonly ILiaisonEnseignantService _liaisonService;

    public LiaisonEnseignantController(ILiaisonEnseignantService liaisonService)
    {
        _liaisonService = liaisonService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

    /// <summary>
    /// Liste les liaisons enseignant (filtrable par user_id ou company_id).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<LiaisonEnseignantDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLiaisons(
        [FromQuery] int? userId = null,
        [FromQuery] int? companyId = null,
        CancellationToken ct = default)
    {
        var items = await _liaisonService.GetLiaisonsAsync(userId, companyId, ct);
        return Ok(ApiResponse<List<LiaisonEnseignantDto>>.Ok(items));
    }

    /// <summary>
    /// Cree une liaison enseignant (scan QR ou identifiant).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LiaisonEnseignantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateLiaison([FromBody] CreateLiaisonEnseignantRequest request, CancellationToken ct)
    {
        var liaison = await _liaisonService.CreateLiaisonAsync(GetUserId(), request, ct);
        return Ok(ApiResponse<LiaisonEnseignantDto>.Ok(liaison, "Demande de liaison creee."));
    }

    /// <summary>
    /// Accepte une liaison en attente.
    /// </summary>
    [HttpPut("{id:int}/accept")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptLiaison(int id, CancellationToken ct)
    {
        await _liaisonService.AcceptLiaisonAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Liaison acceptee."));
    }

    /// <summary>
    /// Rejette une liaison en attente.
    /// </summary>
    [HttpPut("{id:int}/reject")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectLiaison(int id, CancellationToken ct)
    {
        await _liaisonService.RejectLiaisonAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Liaison rejetee."));
    }

    /// <summary>
    /// Termine une liaison acceptee (conserve l'historique).
    /// </summary>
    [HttpPut("{id:int}/terminate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TerminateLiaison(int id, CancellationToken ct)
    {
        await _liaisonService.TerminateLiaisonAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Liaison terminee."));
    }

    /// <summary>
    /// Reintegre une liaison terminee.
    /// </summary>
    [HttpPut("{id:int}/reintegrate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReintegrateLiaison(int id, CancellationToken ct)
    {
        await _liaisonService.ReintegrateLiaisonAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Liaison reintegree."));
    }
}
