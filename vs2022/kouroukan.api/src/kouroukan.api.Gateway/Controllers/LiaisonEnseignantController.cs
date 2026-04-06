using System.Security.Claims;
using Dapper;
using GnDapper.Connection;
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
    private readonly IEmailService _emailService;
    private readonly IDbConnectionFactory _connectionFactory;

    public LiaisonEnseignantController(
        ILiaisonEnseignantService liaisonService,
        IEmailService emailService,
        IDbConnectionFactory connectionFactory)
    {
        _liaisonService = liaisonService;
        _emailService = emailService;
        _connectionFactory = connectionFactory;
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

        // Notifier le directeur de l'etablissement (fire-and-forget)
        _ = Task.Run(async () =>
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                var director = await conn.QuerySingleOrDefaultAsync<(string Email, string FirstName)>(
                    """
                    SELECT u.email AS Email, u.first_name AS FirstName
                    FROM auth.users u
                    INNER JOIN auth.user_companies uc ON uc.user_id = u.id
                    WHERE uc.company_id = @CompanyId AND uc.role = 'owner' AND uc.is_deleted = FALSE AND u.is_deleted = FALSE
                    LIMIT 1
                    """,
                    new { liaison.CompanyId });
                if (!string.IsNullOrWhiteSpace(director.Email))
                {
                    await _emailService.SendLiaisonRequestEmailAsync(
                        director.Email, director.FirstName, liaison.UserNom, liaison.CompanyNom);
                }
            }
            catch { /* logged in EmailService */ }
        });

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

        // Notifier l'enseignant (fire-and-forget)
        _ = NotifyTeacherForLiaisonAsync(id, (email, name, company) =>
            _emailService.SendLiaisonAcceptedEmailAsync(email, name, company));

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

        // Notifier l'enseignant (fire-and-forget)
        _ = NotifyTeacherForLiaisonAsync(id, (email, name, company) =>
            _emailService.SendLiaisonRejectedEmailAsync(email, name, company));

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

        // Notifier l'enseignant (fire-and-forget)
        _ = NotifyTeacherForLiaisonAsync(id, (email, name, company) =>
            _emailService.SendLiaisonTerminatedEmailAsync(email, name, company));

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

    /// <summary>
    /// Helper : recupere les infos enseignant+etablissement pour une liaison et envoie l'email.
    /// </summary>
    private async Task NotifyTeacherForLiaisonAsync(int liaisonId, Func<string, string, string, Task> sendEmail)
    {
        try
        {
            using var conn = _connectionFactory.CreateConnection();
            var info = await conn.QuerySingleOrDefaultAsync<(string Email, string FirstName, string CompanyName)>(
                """
                SELECT u.email AS Email, u.first_name AS FirstName, c.name AS CompanyName
                FROM auth.liaisons_enseignant le
                INNER JOIN auth.users u ON u.id = le.user_id
                INNER JOIN auth.companies c ON c.id = le.company_id
                WHERE le.id = @Id
                """,
                new { Id = liaisonId });

            if (!string.IsNullOrWhiteSpace(info.Email))
            {
                await sendEmail(info.Email, info.FirstName, info.CompanyName);
            }
        }
        catch { /* logged in EmailService */ }
    }
}
