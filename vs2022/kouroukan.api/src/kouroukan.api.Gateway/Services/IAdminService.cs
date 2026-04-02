using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Service d'administration de la plateforme Kouroukan.
/// Gere les forfaits, abonnements, gestes commerciaux, configurations et etablissements.
/// </summary>
public interface IAdminService
{
    // ─── Forfaits ──────────────────────────────────────────────────────────────

    Task<PagedResult<ForfaitDto>> GetForfaitsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<ForfaitDto?> GetForfaitByIdAsync(int id, CancellationToken ct = default);
    Task<ForfaitDto> CreateForfaitAsync(CreateForfaitRequest request, CancellationToken ct = default);
    Task UpdateForfaitAsync(int id, UpdateForfaitRequest request, CancellationToken ct = default);
    Task DeleteForfaitAsync(int id, CancellationToken ct = default);
    Task UpdateForfaitTarifAsync(int id, UpdateForfaitTarifRequest request, CancellationToken ct = default);

    // ─── Abonnements ───────────────────────────────────────────────────────────

    Task<PagedResult<AbonnementDto>> GetAbonnementsAsync(int page, int pageSize, int? companyId, int? userId, CancellationToken ct = default);
    Task<AbonnementDto> CreateAbonnementAsync(CreateAbonnementRequest request, CancellationToken ct = default);
    Task UpdateAbonnementAsync(int id, UpdateAbonnementRequest request, CancellationToken ct = default);
    Task DeleteAbonnementAsync(int id, CancellationToken ct = default);

    // ─── Gestes Commerciaux ────────────────────────────────────────────────────

    Task<List<GesteCommercialDto>> GetGestesCommerciauxAsync(string? typeCible, int? companyId, CancellationToken ct = default);
    Task<GesteCommercialDto> CreateGesteCommercialAsync(CreateGesteCommercialRequest request, CancellationToken ct = default);
    Task UpdateGesteCommercialAsync(int id, UpdateGesteCommercialRequest request, CancellationToken ct = default);
    Task DeleteGesteCommercialAsync(int id, CancellationToken ct = default);

    // ─── Email Config ──────────────────────────────────────────────────────────

    Task<EmailConfigDto?> GetEmailConfigAsync(CancellationToken ct = default);
    Task UpdateEmailConfigAsync(UpdateEmailConfigRequest request, CancellationToken ct = default);
    Task<bool> SendTestEmailAsync(TestEmailRequest request, CancellationToken ct = default);

    // ─── SMS Config ────────────────────────────────────────────────────────────

    Task<SmsConfigDto?> GetSmsConfigAsync(CancellationToken ct = default);
    Task UpdateSmsConfigAsync(UpdateSmsConfigRequest request, CancellationToken ct = default);

    // ─── Comptes Mobile Money ──────────────────────────────────────────────────

    Task<List<CompteMobileDto>> GetComptesMobileAsync(CancellationToken ct = default);
    Task<CompteMobileDto> CreateCompteMobileAsync(CreateCompteMobileRequest request, CancellationToken ct = default);
    Task UpdateCompteMobileAsync(int id, UpdateCompteMobileRequest request, CancellationToken ct = default);
    Task DeleteCompteMobileAsync(int id, CancellationToken ct = default);

    // ─── Contenu IA ────────────────────────────────────────────────────────────

    Task<List<ContenuIaDto>> GetContenusIaAsync(string? rubrique, CancellationToken ct = default);
    Task<ContenuIaDto?> GetContenuIaByIdAsync(int id, CancellationToken ct = default);
    Task<ContenuIaDto> CreateContenuIaAsync(CreateContenuIaRequest request, CancellationToken ct = default);
    Task UpdateContenuIaAsync(int id, UpdateContenuIaRequest request, CancellationToken ct = default);
    Task DeleteContenuIaAsync(int id, CancellationToken ct = default);

    // ─── Statistiques Forfaits ────────────────────────────────────────────────

    Task<ForfaitStatsDto> GetForfaitStatsAsync(CancellationToken ct = default);

    // ─── Etablissements ────────────────────────────────────────────────────────

    Task<PagedResult<AdminEtablissementDto>> GetEtablissementsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<AdminEtablissementDetailDto?> GetEtablissementByIdAsync(int id, CancellationToken ct = default);
    Task<AdminEtablissementDetailDto> UpdateEtablissementAsync(int id, UpdateEtablissementRequest request, CancellationToken ct = default);
    Task DeleteEtablissementAsync(int id, CancellationToken ct = default);
}
