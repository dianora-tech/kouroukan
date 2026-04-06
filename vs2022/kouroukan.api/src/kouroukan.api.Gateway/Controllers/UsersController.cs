using System.Security.Claims;
using Dapper;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kouroukan.Api.Gateway.Controllers;

/// <summary>
/// Gestion des utilisateurs de l'etablissement.
/// Le directeur peut creer, lister et supprimer des comptes pour son personnel.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly IDbConnectionFactory _connectionFactory;

    public UsersController(IUserService userService, IEmailService emailService, IDbConnectionFactory connectionFactory)
    {
        _userService = userService;
        _emailService = emailService;
        _connectionFactory = connectionFactory;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

    /// <summary>
    /// Liste les utilisateurs de l'etablissement du directeur connecte.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:users:manage")]
    [ProducesResponseType(typeof(ApiResponse<List<UserListItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var users = await _userService.GetUsersForCompanyAsync(GetUserId(), ct);
        return Ok(ApiResponse<List<UserListItemDto>>.Ok(users));
    }

    /// <summary>
    /// Cree un nouvel utilisateur dans l'etablissement.
    /// Genere un mot de passe temporaire retourne dans la reponse.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:users:manage")]
    [ProducesResponseType(typeof(ApiResponse<CreateUserResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var result = await _userService.CreateUserAsync(GetUserId(), request, ct);

        // Email de credentials au nouvel utilisateur (fire-and-forget)
        if (!string.IsNullOrWhiteSpace(request.Email) && !string.IsNullOrEmpty(result.TemporaryPassword))
        {
            using var conn = _connectionFactory.CreateConnection();
            var companyName = await conn.ExecuteScalarAsync<string>(
                """
                SELECT c.name FROM auth.companies c
                INNER JOIN auth.user_companies uc ON uc.company_id = c.id
                WHERE uc.user_id = @DirectorId AND uc.role = 'owner' AND uc.is_deleted = FALSE
                LIMIT 1
                """,
                new { DirectorId = GetUserId() }) ?? "Kouroukan";

            _ = _emailService.SendAccountCreatedEmailAsync(
                request.Email,
                request.FirstName,
                result.TemporaryPassword,
                companyName,
                request.Role,
                ct);
        }

        return Ok(ApiResponse<CreateUserResultDto>.Ok(result, "Utilisateur cree avec succes."));
    }

    /// <summary>
    /// Recherche un utilisateur existant par telephone ou email (pour lier un fondateur).
    /// </summary>
    [HttpGet("search")]
    [Authorize(Policy = "RequirePermission:users:manage")]
    [ProducesResponseType(typeof(ApiResponse<UserSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(ApiResponse<object>.Fail("Le parametre de recherche est requis."));

        var user = await _userService.SearchUserAsync(q, ct);
        if (user is null)
            return NotFound(ApiResponse<object>.Fail("Aucun utilisateur trouve."));

        return Ok(ApiResponse<UserSearchResultDto>.Ok(user));
    }

    /// <summary>
    /// Retourne les etablissements lies a l'utilisateur connecte.
    /// Accessible a tout utilisateur authentifie (pas besoin de users:manage).
    /// </summary>
    [HttpGet("companies")]
    [ProducesResponseType(typeof(ApiResponse<List<CompanyDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Companies(CancellationToken ct)
    {
        var companies = await _userService.GetCompaniesForUserAsync(GetUserId(), ct);
        return Ok(ApiResponse<List<CompanyDto>>.Ok(companies));
    }

    /// <summary>
    /// Supprime (soft-delete) un utilisateur de l'etablissement.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequirePermission:users:manage")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _userService.DeleteUserFromCompanyAsync(GetUserId(), id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Utilisateur supprime."));
    }
}
