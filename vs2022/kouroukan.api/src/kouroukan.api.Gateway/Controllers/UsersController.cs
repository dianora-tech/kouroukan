using System.Security.Claims;
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
[Authorize(Policy = "RequirePermission:users:manage")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

    /// <summary>
    /// Liste les utilisateurs de l'etablissement du directeur connecte.
    /// </summary>
    [HttpGet]
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
    [ProducesResponseType(typeof(ApiResponse<CreateUserResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var result = await _userService.CreateUserAsync(GetUserId(), request, ct);
        return Ok(ApiResponse<CreateUserResultDto>.Ok(result, "Utilisateur cree avec succes."));
    }

    /// <summary>
    /// Recherche un utilisateur existant par telephone ou email (pour lier un fondateur).
    /// </summary>
    [HttpGet("search")]
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
    /// </summary>
    [HttpGet("companies")]
    [Authorize]
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
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _userService.DeleteUserFromCompanyAsync(GetUserId(), id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Utilisateur supprime."));
    }
}
