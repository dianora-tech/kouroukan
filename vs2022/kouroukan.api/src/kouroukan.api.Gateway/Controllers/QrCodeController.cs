using System.Security.Claims;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kouroukan.Api.Gateway.Controllers;

/// <summary>
/// Controleur de gestion des QR codes utilisateur.
/// Permet de generer et resoudre des QR codes.
/// </summary>
[ApiController]
[Route("api/auth/qr")]
public sealed class QrCodeController : ControllerBase
{
    private readonly IQrCodeService _qrCodeService;

    public QrCodeController(IQrCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

    /// <summary>
    /// Recupere le QR code de l'utilisateur connecte. Le cree s'il n'existe pas.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<QrCodeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyQrCode(CancellationToken ct)
    {
        var qrCode = await _qrCodeService.GetOrCreateQrCodeAsync(GetUserId(), ct);
        return Ok(ApiResponse<QrCodeDto>.Ok(qrCode));
    }

    /// <summary>
    /// Resout un QR code vers un profil utilisateur.
    /// Accessible a tout utilisateur authentifie.
    /// </summary>
    [HttpGet("resolve/{code}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<QrCodeResolvedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveQrCode(string code, CancellationToken ct)
    {
        var resolved = await _qrCodeService.ResolveQrCodeAsync(code, ct);
        if (resolved is null)
            return NotFound(ApiResponse<object>.Fail("QR code invalide ou expire."));

        return Ok(ApiResponse<QrCodeResolvedDto>.Ok(resolved));
    }
}
