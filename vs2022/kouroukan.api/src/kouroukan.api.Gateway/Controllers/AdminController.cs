using Dapper;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kouroukan.Api.Gateway.Controllers;

/// <summary>
/// Controleur d'administration de la plateforme Kouroukan.
/// Gere les forfaits, abonnements, gestes commerciaux, configurations,
/// comptes Mobile Money, contenus IA et etablissements.
/// Tous les endpoints necessitent la permission admin:manage.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Policy = "RequirePermission:admin:manage")]
public sealed class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IEmailService _emailService;
    private readonly IDbConnectionFactory _connectionFactory;

    public AdminController(IAdminService adminService, IEmailService emailService, IDbConnectionFactory connectionFactory)
    {
        _adminService = adminService;
        _emailService = emailService;
        _connectionFactory = connectionFactory;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // FORFAITS
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Liste les forfaits (pagine).
    /// </summary>
    [HttpGet("forfaits")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ForfaitDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetForfaits([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _adminService.GetForfaitsAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<ForfaitDto>>.Ok(result));
    }

    /// <summary>
    /// Recupere un forfait par son identifiant.
    /// </summary>
    [HttpGet("forfaits/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ForfaitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetForfait(int id, CancellationToken ct)
    {
        var forfait = await _adminService.GetForfaitByIdAsync(id, ct);
        if (forfait is null)
            return NotFound(ApiResponse<object>.Fail("Forfait introuvable."));

        return Ok(ApiResponse<ForfaitDto>.Ok(forfait));
    }

    /// <summary>
    /// Cree un nouveau forfait.
    /// </summary>
    [HttpPost("forfaits")]
    [ProducesResponseType(typeof(ApiResponse<ForfaitDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateForfait([FromBody] CreateForfaitRequest request, CancellationToken ct)
    {
        var forfait = await _adminService.CreateForfaitAsync(request, ct);
        return Ok(ApiResponse<ForfaitDto>.Ok(forfait, "Forfait cree avec succes."));
    }

    /// <summary>
    /// Met a jour un forfait.
    /// </summary>
    [HttpPut("forfaits/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateForfait(int id, [FromBody] UpdateForfaitRequest request, CancellationToken ct)
    {
        await _adminService.UpdateForfaitAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Forfait mis a jour."));
    }

    /// <summary>
    /// Supprime un forfait (soft delete).
    /// </summary>
    [HttpDelete("forfaits/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteForfait(int id, CancellationToken ct)
    {
        await _adminService.DeleteForfaitAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Forfait supprime."));
    }

    /// <summary>
    /// Met a jour le tarif d'un forfait avec une date d'effet.
    /// Insere un enregistrement dans auth.forfait_tarifs.
    /// </summary>
    [HttpPut("forfaits/{id:int}/tarif")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateForfaitTarif(int id, [FromBody] UpdateForfaitTarifRequest request, CancellationToken ct)
    {
        await _adminService.UpdateForfaitTarifAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Tarif mis a jour."));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // ABONNEMENTS
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Liste les abonnements (pagine, filtrable par type_cible).
    /// </summary>
    [HttpGet("abonnements")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AbonnementDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAbonnements(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? companyId = null,
        [FromQuery] int? userId = null,
        CancellationToken ct = default)
    {
        var result = await _adminService.GetAbonnementsAsync(page, pageSize, companyId, userId, ct);
        return Ok(ApiResponse<PagedResult<AbonnementDto>>.Ok(result));
    }

    /// <summary>
    /// Cree un nouvel abonnement.
    /// </summary>
    [HttpPost("abonnements")]
    [ProducesResponseType(typeof(ApiResponse<AbonnementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAbonnement([FromBody] CreateAbonnementRequest request, CancellationToken ct)
    {
        var abonnement = await _adminService.CreateAbonnementAsync(request, ct);

        // Notifier l'utilisateur/etablissement (fire-and-forget)
        _ = NotifyAbonnementOwnerAsync(abonnement.CompanyId, abonnement.UserId, abonnement.ForfaitNom,
            (email, name, plan) => _emailService.SendAdminSubscriptionCreatedEmailAsync(
                email, name, plan, abonnement.DateDebut.ToString("dd/MM/yyyy")));

        return Ok(ApiResponse<AbonnementDto>.Ok(abonnement, "Abonnement cree avec succes."));
    }

    /// <summary>
    /// Met a jour un abonnement.
    /// </summary>
    [HttpPut("abonnements/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAbonnement(int id, [FromBody] UpdateAbonnementRequest request, CancellationToken ct)
    {
        await _adminService.UpdateAbonnementAsync(id, request, ct);

        // Notifier l'utilisateur/etablissement (fire-and-forget)
        _ = Task.Run(async () =>
        {
            var info = await GetAbonnementInfoAsync(id);
            if (info.HasValue)
                await NotifyAbonnementOwnerAsync(info.Value.CompanyId, info.Value.UserId, info.Value.ForfaitNom,
                    (email, name, plan) => _emailService.SendAdminSubscriptionUpdatedEmailAsync(email, name, plan));
        });

        return Ok(ApiResponse<object>.Ok(null!, "Abonnement mis a jour."));
    }

    /// <summary>
    /// Supprime un abonnement (soft delete).
    /// </summary>
    [HttpDelete("abonnements/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAbonnement(int id, CancellationToken ct)
    {
        // Recuperer les infos AVANT suppression pour l'email
        var abonnementInfo = await GetAbonnementInfoAsync(id);

        await _adminService.DeleteAbonnementAsync(id, ct);

        // Notifier l'utilisateur/etablissement (fire-and-forget)
        if (abonnementInfo.HasValue)
        {
            _ = NotifyAbonnementOwnerAsync(abonnementInfo.Value.CompanyId, abonnementInfo.Value.UserId, abonnementInfo.Value.ForfaitNom,
                (email, name, plan) => _emailService.SendAdminSubscriptionDeletedEmailAsync(email, name, plan));
        }

        return Ok(ApiResponse<object>.Ok(null!, "Abonnement supprime."));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // GESTES COMMERCIAUX
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Liste les gestes commerciaux (filtrable par type_cible et company_id).
    /// </summary>
    [HttpGet("gestes-commerciaux")]
    [ProducesResponseType(typeof(ApiResponse<List<GesteCommercialDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGestesCommerciaux(
        [FromQuery] string? typeCible = null,
        [FromQuery] int? companyId = null,
        CancellationToken ct = default)
    {
        var items = await _adminService.GetGestesCommerciauxAsync(typeCible, companyId, ct);
        return Ok(ApiResponse<List<GesteCommercialDto>>.Ok(items));
    }

    /// <summary>
    /// Cree un nouveau geste commercial.
    /// </summary>
    [HttpPost("gestes-commerciaux")]
    [ProducesResponseType(typeof(ApiResponse<GesteCommercialDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateGesteCommercial([FromBody] CreateGesteCommercialRequest request, CancellationToken ct)
    {
        var geste = await _adminService.CreateGesteCommercialAsync(request, ct);
        return Ok(ApiResponse<GesteCommercialDto>.Ok(geste, "Geste commercial cree avec succes."));
    }

    /// <summary>
    /// Met a jour un geste commercial.
    /// </summary>
    [HttpPut("gestes-commerciaux/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateGesteCommercial(int id, [FromBody] UpdateGesteCommercialRequest request, CancellationToken ct)
    {
        await _adminService.UpdateGesteCommercialAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Geste commercial mis a jour."));
    }

    /// <summary>
    /// Supprime un geste commercial (soft delete).
    /// </summary>
    [HttpDelete("gestes-commerciaux/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteGesteCommercial(int id, CancellationToken ct)
    {
        await _adminService.DeleteGesteCommercialAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Geste commercial supprime."));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // EMAIL CONFIG
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Recupere la configuration email actuelle.
    /// </summary>
    [HttpGet("email-config")]
    [ProducesResponseType(typeof(ApiResponse<EmailConfigDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmailConfig(CancellationToken ct)
    {
        var config = await _adminService.GetEmailConfigAsync(ct);
        if (config is null)
            return NotFound(ApiResponse<object>.Fail("Aucune configuration email trouvee."));

        return Ok(ApiResponse<EmailConfigDto>.Ok(config));
    }

    /// <summary>
    /// Met a jour la configuration email.
    /// </summary>
    [HttpPut("email-config")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateEmailConfig([FromBody] UpdateEmailConfigRequest request, CancellationToken ct)
    {
        await _adminService.UpdateEmailConfigAsync(request, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Configuration email mise a jour."));
    }

    /// <summary>
    /// Envoie un email de test.
    /// </summary>
    [HttpPost("email-config/test")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendTestEmail([FromBody] TestEmailRequest request, CancellationToken ct)
    {
        var success = await _adminService.SendTestEmailAsync(request, ct);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail("Echec de l'envoi de l'email de test. Verifiez la configuration SMTP."));

        return Ok(ApiResponse<object>.Ok(null!, "Email de test envoye avec succes."));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SMS CONFIG
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Recupere la configuration SMS actuelle avec le solde.
    /// </summary>
    [HttpGet("sms-config")]
    [ProducesResponseType(typeof(ApiResponse<SmsConfigDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSmsConfig(CancellationToken ct)
    {
        var config = await _adminService.GetSmsConfigAsync(ct);
        if (config is null)
            return NotFound(ApiResponse<object>.Fail("Aucune configuration SMS trouvee."));

        return Ok(ApiResponse<SmsConfigDto>.Ok(config));
    }

    /// <summary>
    /// Met a jour la configuration SMS.
    /// </summary>
    [HttpPut("sms-config")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSmsConfig([FromBody] UpdateSmsConfigRequest request, CancellationToken ct)
    {
        await _adminService.UpdateSmsConfigAsync(request, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Configuration SMS mise a jour."));
    }

    /// <summary>
    /// Envoie un SMS de test via NimbaSMS.
    /// </summary>
    [HttpPost("sms-config/test")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendTestSms([FromBody] TestSmsRequest request, CancellationToken ct)
    {
        var success = await _adminService.SendTestSmsAsync(request, ct);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail("Echec de l'envoi du SMS. Verifiez la configuration NimbaSMS."));
        return Ok(ApiResponse<object>.Ok(null!, "SMS de test envoye avec succes."));
    }

    /// <summary>
    /// Synchronise le solde NimbaSMS.
    /// </summary>
    [HttpPost("sms-config/sync-balance")]
    [ProducesResponseType(typeof(ApiResponse<SmsConfigDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncSmsBalance(CancellationToken ct)
    {
        await _adminService.SyncSmsBalanceAsync(ct);
        var config = await _adminService.GetSmsConfigAsync(ct);
        return Ok(ApiResponse<SmsConfigDto>.Ok(config!));
    }

    /// <summary>
    /// Historique des SMS envoyes (pagine).
    /// </summary>
    [HttpGet("sms-config/historique")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SmsHistoriqueDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSmsHistorique([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _adminService.GetSmsHistoriqueAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<SmsHistoriqueDto>>.Ok(result));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // COMPTES MOBILE MONEY
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Liste les comptes Mobile Money admin.
    /// </summary>
    [HttpGet("comptes-mobile")]
    [ProducesResponseType(typeof(ApiResponse<List<CompteMobileDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComptesMobile(CancellationToken ct)
    {
        var items = await _adminService.GetComptesMobileAsync(ct);
        return Ok(ApiResponse<List<CompteMobileDto>>.Ok(items));
    }

    /// <summary>
    /// Ajoute un compte Mobile Money.
    /// </summary>
    [HttpPost("comptes-mobile")]
    [ProducesResponseType(typeof(ApiResponse<CompteMobileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCompteMobile([FromBody] CreateCompteMobileRequest request, CancellationToken ct)
    {
        var compte = await _adminService.CreateCompteMobileAsync(request, ct);
        return Ok(ApiResponse<CompteMobileDto>.Ok(compte, "Compte Mobile Money ajoute."));
    }

    /// <summary>
    /// Met a jour un compte Mobile Money.
    /// </summary>
    [HttpPut("comptes-mobile/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCompteMobile(int id, [FromBody] UpdateCompteMobileRequest request, CancellationToken ct)
    {
        await _adminService.UpdateCompteMobileAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Compte Mobile Money mis a jour."));
    }

    /// <summary>
    /// Supprime un compte Mobile Money (soft delete).
    /// </summary>
    [HttpDelete("comptes-mobile/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteCompteMobile(int id, CancellationToken ct)
    {
        await _adminService.DeleteCompteMobileAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Compte Mobile Money supprime."));
    }

    /// <summary>
    /// Historique des transactions Mobile Money (pagine).
    /// </summary>
    [HttpGet("comptes-mobile/transactions")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TransactionMobileDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactionsMobile([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _adminService.GetTransactionsMobileAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<TransactionMobileDto>>.Ok(result));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // CONTENU IA
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Liste les contenus IA (filtrable par rubrique).
    /// </summary>
    [HttpGet("contenu-ia")]
    [ProducesResponseType(typeof(ApiResponse<List<ContenuIaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContenusIa([FromQuery] string? rubrique = null, CancellationToken ct = default)
    {
        var items = await _adminService.GetContenusIaAsync(rubrique, ct);
        return Ok(ApiResponse<List<ContenuIaDto>>.Ok(items));
    }

    /// <summary>
    /// Recupere un contenu IA par son identifiant.
    /// </summary>
    [HttpGet("contenu-ia/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ContenuIaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContenuIa(int id, CancellationToken ct)
    {
        var contenu = await _adminService.GetContenuIaByIdAsync(id, ct);
        if (contenu is null)
            return NotFound(ApiResponse<object>.Fail("Contenu IA introuvable."));

        return Ok(ApiResponse<ContenuIaDto>.Ok(contenu));
    }

    /// <summary>
    /// Cree un nouveau contenu IA.
    /// </summary>
    [HttpPost("contenu-ia")]
    [ProducesResponseType(typeof(ApiResponse<ContenuIaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateContenuIa([FromBody] CreateContenuIaRequest request, CancellationToken ct)
    {
        var contenu = await _adminService.CreateContenuIaAsync(request, ct);
        return Ok(ApiResponse<ContenuIaDto>.Ok(contenu, "Contenu IA cree avec succes."));
    }

    /// <summary>
    /// Met a jour un contenu IA.
    /// </summary>
    [HttpPut("contenu-ia/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateContenuIa(int id, [FromBody] UpdateContenuIaRequest request, CancellationToken ct)
    {
        await _adminService.UpdateContenuIaAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Contenu IA mis a jour."));
    }

    /// <summary>
    /// Supprime un contenu IA (soft delete).
    /// </summary>
    [HttpDelete("contenu-ia/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteContenuIa(int id, CancellationToken ct)
    {
        await _adminService.DeleteContenuIaAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Contenu IA supprime."));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // ETABLISSEMENTS
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Liste tous les etablissements avec nombre d'utilisateurs et forfait.
    /// </summary>
    [HttpGet("etablissements")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminEtablissementDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEtablissements([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _adminService.GetEtablissementsAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<AdminEtablissementDto>>.Ok(result));
    }

    /// <summary>
    /// Recupere le detail d'un etablissement.
    /// </summary>
    [HttpGet("etablissements/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AdminEtablissementDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEtablissement(int id, CancellationToken ct)
    {
        var etablissement = await _adminService.GetEtablissementByIdAsync(id, ct);
        if (etablissement is null)
            return NotFound(ApiResponse<object>.Fail("Etablissement introuvable."));

        return Ok(ApiResponse<AdminEtablissementDetailDto>.Ok(etablissement));
    }

    /// <summary>
    /// Met a jour les metadonnees d'un etablissement.
    /// </summary>
    [HttpPut("etablissements/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AdminEtablissementDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateEtablissement(int id, [FromBody] UpdateEtablissementRequest request, CancellationToken ct)
    {
        var updated = await _adminService.UpdateEtablissementAsync(id, request, ct);
        return Ok(ApiResponse<AdminEtablissementDetailDto>.Ok(updated, "Etablissement mis a jour."));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // STATISTIQUES FORFAITS
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Recupere les statistiques globales des forfaits.
    /// </summary>
    [HttpGet("stats/forfaits")]
    [ProducesResponseType(typeof(ApiResponse<ForfaitStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetForfaitStats(CancellationToken ct)
    {
        var stats = await _adminService.GetForfaitStatsAsync(ct);
        return Ok(ApiResponse<ForfaitStatsDto>.Ok(stats));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // DASHBOARD STATS
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Recupere les KPIs du tableau de bord (etablissements, enseignants, parents, eleves).
    /// </summary>
    [HttpGet("stats/dashboard")]
    [ProducesResponseType(typeof(ApiResponse<DashboardKpiDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardKpis(CancellationToken ct)
    {
        var kpis = await _adminService.GetDashboardKpisAsync(ct);
        return Ok(ApiResponse<DashboardKpiDto>.Ok(kpis));
    }

    /// <summary>
    /// Recupere les revenus mensuels des 6 derniers mois.
    /// </summary>
    [HttpGet("stats/revenus")]
    [ProducesResponseType(typeof(ApiResponse<List<RevenuMensuelDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenusMensuels(CancellationToken ct)
    {
        var revenus = await _adminService.GetRevenusMensuelsAsync(ct);
        return Ok(ApiResponse<List<RevenuMensuelDto>>.Ok(revenus));
    }

    /// <summary>
    /// Recupere la repartition des etablissements par region.
    /// </summary>
    [HttpGet("stats/regions")]
    [ProducesResponseType(typeof(ApiResponse<List<RegionStatDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegionStats(CancellationToken ct)
    {
        var regions = await _adminService.GetRegionStatsAsync(ct);
        return Ok(ApiResponse<List<RegionStatDto>>.Ok(regions));
    }

    /// <summary>
    /// Recupere les statistiques d'usage de la plateforme.
    /// </summary>
    [HttpGet("stats/usage")]
    [ProducesResponseType(typeof(ApiResponse<List<UsageStatDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsageStats(CancellationToken ct)
    {
        var usage = await _adminService.GetUsageStatsAsync(ct);
        return Ok(ApiResponse<List<UsageStatDto>>.Ok(usage));
    }

    /// <summary>Supprime (soft delete) un etablissement.</summary>
    [HttpDelete("etablissements/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteEtablissement(int id, CancellationToken ct)
    {
        await _adminService.DeleteEtablissementAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Etablissement supprime."));
    }

    // ════════════════════════════════��══════════════════════════════════════════
    // HELPERS EMAIL ABONNEMENT
    // ════════════════════���══════════════════════════════════════════════════════

    /// <summary>
    /// Recupere les infos d'un abonnement (company_id, user_id, forfait_nom) pour les notifications.
    /// </summary>
    private async Task<(int? CompanyId, int? UserId, string ForfaitNom)?> GetAbonnementInfoAsync(int abonnementId)
    {
        try
        {
            using var conn = _connectionFactory.CreateConnection();
            var info = await conn.QuerySingleOrDefaultAsync<(int? CompanyId, int? UserId, string ForfaitNom)>(
                """
                SELECT a.company_id AS CompanyId, a.user_id AS UserId,
                       COALESCE(f.nom, 'Forfait') AS ForfaitNom
                FROM forfaits.abonnements a
                LEFT JOIN forfaits.forfaits f ON f.id = a.forfait_id
                WHERE a.id = @Id
                """,
                new { Id = abonnementId });
            return info;
        }
        catch { return null; }
    }

    /// <summary>
    /// Notifie le proprietaire d'un abonnement (owner de la company ou user direct).
    /// </summary>
    private async Task NotifyAbonnementOwnerAsync(int? companyId, int? userId, string forfaitNom, Func<string, string, string, Task> sendEmail)
    {
        try
        {
            using var conn = _connectionFactory.CreateConnection();

            string email;
            string firstName;

            if (companyId.HasValue)
            {
                // Notifier le directeur (owner) de l'etablissement
                var owner = await conn.QuerySingleOrDefaultAsync<(string Email, string FirstName)>(
                    """
                    SELECT u.email AS Email, u.first_name AS FirstName
                    FROM auth.users u
                    INNER JOIN auth.user_companies uc ON uc.user_id = u.id
                    WHERE uc.company_id = @CompanyId AND uc.role = 'owner' AND uc.is_deleted = FALSE AND u.is_deleted = FALSE
                    LIMIT 1
                    """,
                    new { CompanyId = companyId.Value });
                email = owner.Email;
                firstName = owner.FirstName;
            }
            else if (userId.HasValue)
            {
                // Notifier l'utilisateur directement
                var user = await conn.QuerySingleOrDefaultAsync<(string Email, string FirstName)>(
                    "SELECT email AS Email, first_name AS FirstName FROM auth.users WHERE id = @Id AND is_deleted = FALSE",
                    new { Id = userId.Value });
                email = user.Email;
                firstName = user.FirstName;
            }
            else return;

            if (!string.IsNullOrWhiteSpace(email))
            {
                await sendEmail(email, firstName, forfaitNom);
            }
        }
        catch { /* logged in EmailService */ }
    }
}
