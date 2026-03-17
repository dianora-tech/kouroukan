using System.Threading.RateLimiting;
using GnSecurity.Jwt;
using Kouroukan.Api.Gateway.Auth;
using Kouroukan.Api.Gateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Kouroukan.Api.Gateway.Controllers;

/// <summary>
/// Controleur d'authentification.
/// Gere le login, le rafraichissement de tokens, le logout, le profil utilisateur et les CGU.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="AuthController"/>.
    /// </summary>
    public AuthController(
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Authentifie un utilisateur et retourne les tokens JWT.
    /// </summary>
    /// <param name="request">Email et mot de passe.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Access token et refresh token.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokensDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var tokens = await _tokenService.LoginAsync(request.Email, request.Password, cancellationToken);

        return Ok(ApiResponse<AuthTokensDto>.Ok(tokens, "Connexion reussie."));
    }

    /// <summary>
    /// Rafraichit les tokens a partir d'un refresh token valide.
    /// Implemente la rotation de tokens avec detection de reutilisation.
    /// </summary>
    /// <param name="request">Refresh token a echanger.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Nouveaux access token et refresh token.</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthTokensDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var tokenResult = await _refreshTokenService.RefreshAsync(request.RefreshToken, cancellationToken);

        var dto = new AuthTokensDto
        {
            AccessToken = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken,
            AccessTokenExpiresAt = tokenResult.AccessTokenExpiresAt,
            RefreshTokenExpiresAt = tokenResult.RefreshTokenExpiresAt
        };

        return Ok(ApiResponse<AuthTokensDto>.Ok(dto, "Tokens rafraichis."));
    }

    /// <summary>
    /// Revoque le refresh token de l'utilisateur (deconnexion).
    /// </summary>
    /// <param name="request">Refresh token a revoquer.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        await _refreshTokenService.RevokeAsync(request.RefreshToken, cancellationToken);

        _logger.LogInformation("Deconnexion de l'utilisateur {UserId}",
            User.FindFirst("sub")?.Value);

        return Ok(ApiResponse<object>.Ok(null!, "Deconnexion reussie."));
    }

    /// <summary>
    /// Retourne le profil de l'utilisateur authentifie.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Profil utilisateur avec roles, permissions et statut CGU.</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        var profile = await _tokenService.GetUserProfileAsync(userId, cancellationToken);
        if (profile is null)
            return NotFound(ApiResponse<object>.Fail("Utilisateur introuvable."));

        return Ok(ApiResponse<UserProfileDto>.Ok(profile));
    }

    /// <summary>
    /// Retourne la version active des CGU (contenu Markdown).
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Version, contenu et date de publication des CGU.</returns>
    [HttpGet("cgu/active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CguVersionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveCgu(CancellationToken cancellationToken)
    {
        var cgu = await _tokenService.GetActiveCguAsync(cancellationToken);
        if (cgu is null)
            return NotFound(ApiResponse<object>.Fail("Aucune version de CGU active."));

        return Ok(ApiResponse<CguVersionDto>.Ok(cgu));
    }

    /// <summary>
    /// Enregistre l'acceptation des CGU par l'utilisateur authentifie.
    /// Retourne un nouveau token avec le claim cguVersion mis a jour.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Nouveaux tokens avec le claim CGU mis a jour.</returns>
    [HttpPost("cgu/accept")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<AuthTokensDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptCgu(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        var activeCgu = await _tokenService.GetActiveCguAsync(cancellationToken);
        if (activeCgu is null)
            return NotFound(ApiResponse<object>.Fail("Aucune version de CGU active."));

        var tokens = await _tokenService.AcceptCguAsync(userId, activeCgu.Version, cancellationToken);

        return Ok(ApiResponse<AuthTokensDto>.Ok(tokens, "CGU acceptees avec succes."));
    }
}
