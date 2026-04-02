using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Service de gestion des forfaits cote utilisateur.
/// Gere le statut d'abonnement, la souscription, la resiliation et le quota d'eleves.
/// </summary>
public interface IForfaitUserService
{
    /// <summary>Recupere le statut de l'abonnement actif.</summary>
    Task<ForfaitStatusDto> GetStatusAsync(int? companyId, int? userId, CancellationToken ct = default);

    /// <summary>Liste les plans disponibles pour un type de cible.</summary>
    Task<List<ForfaitPlanDto>> GetAvailablePlansAsync(string typeCible, CancellationToken ct = default);

    /// <summary>Souscrit a un forfait.</summary>
    Task<AbonnementHistoryDto> SubscribeAsync(int? companyId, int? userId, SubscribeForfaitRequest request, CancellationToken ct = default);

    /// <summary>Resilie un abonnement.</summary>
    Task CancelAsync(int abonnementId, int? companyId, int? userId, CancellationToken ct = default);

    /// <summary>Recupere l'historique des abonnements.</summary>
    Task<List<AbonnementHistoryDto>> GetHistoryAsync(int? companyId, int? userId, CancellationToken ct = default);

    /// <summary>Verifie le quota d'eleves d'un etablissement.</summary>
    Task<QuotaCheckResult> CheckStudentQuotaAsync(int companyId, CancellationToken ct = default);
}
