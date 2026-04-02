using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Service de gestion des liaisons enseignant-etablissement.
/// </summary>
public interface ILiaisonEnseignantService
{
    Task<List<LiaisonEnseignantDto>> GetLiaisonsAsync(int? userId, int? companyId, CancellationToken ct = default);
    Task<LiaisonEnseignantDto> CreateLiaisonAsync(int userId, CreateLiaisonEnseignantRequest request, CancellationToken ct = default);
    Task AcceptLiaisonAsync(int id, CancellationToken ct = default);
    Task RejectLiaisonAsync(int id, CancellationToken ct = default);
    Task TerminateLiaisonAsync(int id, CancellationToken ct = default);
    Task ReintegrateLiaisonAsync(int id, CancellationToken ct = default);
}
