using System.Security.Claims;
using System.Threading.RateLimiting;
using Dapper;
using GnDapper.Connection;
using GnSecurity.Jwt;
using Kouroukan.Api.Gateway.Auth;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
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
    private readonly IMinioStorageService _storageService;
    private readonly IEmailService _emailService;
    private readonly ITurnstileService _turnstileService;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<AuthController> _logger;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };
    private const long MaxAvatarSizeBytes = 2 * 1024 * 1024; // 2 MB

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="AuthController"/>.
    /// </summary>
    public AuthController(
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IMinioStorageService storageService,
        IEmailService emailService,
        ITurnstileService turnstileService,
        IDbConnectionFactory connectionFactory,
        ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _storageService = storageService;
        _emailService = emailService;
        _turnstileService = turnstileService;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <summary>
    /// Inscrit un nouvel etablissement (directeur + company) et retourne les tokens JWT.
    /// </summary>
    /// <param name="request">Donnees d'inscription du wizard portail.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Access token et refresh token.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokensDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        // Validation Cloudflare Turnstile (anti-bot)
        if (!string.IsNullOrEmpty(request.TurnstileToken))
        {
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var isHuman = await _turnstileService.ValidateAsync(request.TurnstileToken, remoteIp, cancellationToken);
            if (!isHuman)
            {
                _logger.LogWarning("Turnstile validation echouee pour inscription depuis {Ip}", remoteIp);
                return BadRequest(ApiResponse<object>.Fail("Verification anti-bot echouee. Veuillez reessayer."));
            }
        }

        var tokens = await _tokenService.RegisterAsync(request, cancellationToken);

        // Email de bienvenue (fire-and-forget)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            _ = _emailService.SendWelcomeEmailAsync(
                request.Email,
                request.FirstName,
                request.SchoolName ?? $"{request.FirstName} {request.LastName}",
                cancellationToken);
        }

        return Ok(ApiResponse<AuthTokensDto>.Ok(tokens, "Inscription reussie."));
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
        // Validation Cloudflare Turnstile (anti-bot)
        if (!string.IsNullOrEmpty(request.TurnstileToken))
        {
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var isHuman = await _turnstileService.ValidateAsync(request.TurnstileToken, remoteIp, cancellationToken);
            if (!isHuman)
            {
                _logger.LogWarning("Turnstile validation echouee pour {Email} depuis {Ip}", request.Email, remoteIp);
                return BadRequest(ApiResponse<object>.Fail("Verification anti-bot echouee. Veuillez reessayer."));
            }
        }

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
    /// Le body est optionnel : si aucun refreshToken n'est fourni,
    /// la deconnexion est effectuee sans revoquer de token specifique.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest? request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst("sub")?.Value;

        if (!string.IsNullOrEmpty(request?.RefreshToken))
        {
            await _refreshTokenService.RevokeAsync(request.RefreshToken, cancellationToken);
        }

        _logger.LogInformation("Deconnexion de l'utilisateur {UserId}", userId);

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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        var activeCgu = await _tokenService.GetActiveCguAsync(cancellationToken);
        if (activeCgu is null)
            return NotFound(ApiResponse<object>.Fail("Aucune version de CGU active."));

        var tokens = await _tokenService.AcceptCguAsync(userId, activeCgu.Version, cancellationToken);

        // Confirmation d'acceptation des CGU (fire-and-forget)
        _ = Task.Run(async () =>
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                var userInfo = await conn.QuerySingleOrDefaultAsync<(string Email, string FirstName)>(
                    "SELECT email AS Email, first_name AS FirstName FROM auth.users WHERE id = @UserId",
                    new { UserId = userId });
                if (!string.IsNullOrWhiteSpace(userInfo.Email))
                {
                    await _emailService.SendCguAcceptedEmailAsync(
                        userInfo.Email, userInfo.FirstName, activeCgu.Version);
                }
            }
            catch { /* logged in EmailService */ }
        });

        return Ok(ApiResponse<AuthTokensDto>.Ok(tokens, "CGU acceptees avec succes."));
    }

    /// <summary>
    /// Change le mot de passe de l'utilisateur authentifie.
    /// Desactive le flag must_change_password apres le changement.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        if (request.NewPassword.Length < 8)
            return BadRequest(ApiResponse<object>.Fail("Le nouveau mot de passe doit contenir au moins 8 caracteres."));

        try
        {
            await _tokenService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);

            // Notification de changement de mot de passe (fire-and-forget)
            using var conn = _connectionFactory.CreateConnection();
            var userInfo = await conn.QuerySingleOrDefaultAsync<(string Email, string FirstName)>(
                "SELECT email AS Email, first_name AS FirstName FROM auth.users WHERE id = @UserId",
                new { UserId = userId });
            if (!string.IsNullOrWhiteSpace(userInfo.Email))
            {
                _ = _emailService.SendPasswordChangedEmailAsync(userInfo.Email, userInfo.FirstName, cancellationToken);
            }

            return Ok(ApiResponse<object>.Ok(null!, "Mot de passe modifie avec succes."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Met a jour les preferences utilisateur (langue, theme).
    /// </summary>
    [HttpPut("preferences")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst("sub")?.Value;

        if (userId is null)
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        await _tokenService.UpdatePreferencesAsync(int.Parse(userId), request.Locale, request.Theme, cancellationToken);

        return Ok(ApiResponse<object>.Ok(null!, "Preferences mises a jour."));
    }

    /// <summary>
    /// Retourne le statut d'onboarding de l'etablissement de l'utilisateur.
    /// </summary>
    [HttpGet("onboarding/status")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OnboardingStatusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOnboardingStatus(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        var status = await _tokenService.GetOnboardingStatusAsync(userId, cancellationToken);
        if (status is null)
            return NotFound(ApiResponse<object>.Fail("Etablissement introuvable."));

        return Ok(ApiResponse<OnboardingStatusDto>.Ok(status));
    }

    /// <summary>
    /// Met a jour l'etape d'onboarding de l'etablissement.
    /// </summary>
    [HttpPut("onboarding/step")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateOnboardingStep([FromBody] UpdateOnboardingStepRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        if (request.Step < 1 || request.Step > 6)
            return BadRequest(ApiResponse<object>.Fail("L'etape doit etre entre 1 et 6."));

        await _tokenService.UpdateOnboardingStepAsync(userId, request.Step, cancellationToken);

        return Ok(ApiResponse<object>.Ok(null!, "Etape d'onboarding mise a jour."));
    }

    /// <summary>
    /// Marque l'onboarding comme termine (skip ou completion).
    /// </summary>
    [HttpPut("onboarding/skip")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SkipOnboarding(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        await _tokenService.CompleteOnboardingAsync(userId, cancellationToken);

        return Ok(ApiResponse<object>.Ok(null!, "Onboarding termine."));
    }

    /// <summary>
    /// Met a jour les informations de l'etablissement.
    /// </summary>
    [HttpPut("company")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        await _tokenService.UpdateCompanyAsync(userId, request, cancellationToken);

        return Ok(ApiResponse<object>.Ok(null!, "Etablissement mis a jour."));
    }

    /// <summary>
    /// Upload ou remplace la photo de profil (avatar) de l'utilisateur.
    /// Formats acceptes : JPEG, PNG, WebP. Taille max : 2 MB.
    /// </summary>
    [HttpPost("avatar")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(MaxAvatarSizeBytes + 1024)] // petite marge pour les headers multipart
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Token invalide."));

        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("Aucun fichier fourni."));

        if (file.Length > MaxAvatarSizeBytes)
            return BadRequest(ApiResponse<object>.Fail("Le fichier ne doit pas depasser 2 MB."));

        if (!AllowedContentTypes.Contains(file.ContentType))
            return BadRequest(ApiResponse<object>.Fail("Format non supporte. Utilisez JPEG, PNG ou WebP."));

        await using var stream = file.OpenReadStream();
        var avatarUrl = await _storageService.UploadAvatarAsync(userId, stream, file.ContentType, file.FileName, cancellationToken);

        // Mettre a jour l'URL en base de donnees
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE auth.users SET avatar_url = @AvatarUrl, updated_at = NOW() WHERE id = @UserId",
            new { AvatarUrl = avatarUrl, UserId = userId });

        _logger.LogInformation("Avatar mis a jour pour l'utilisateur {UserId}", userId);

        return Ok(ApiResponse<object>.Ok(new { avatarUrl }, "Photo de profil mise a jour."));
    }
}
