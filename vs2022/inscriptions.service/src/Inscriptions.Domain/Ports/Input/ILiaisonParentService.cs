using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des liaisons parent.
/// </summary>
public interface ILiaisonParentService
{
    Task<LiaisonParent?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<LiaisonParent>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<LiaisonParent>> GetPagedAsync(int page, int pageSize, int? parentUserId, int? companyId, string? orderBy, CancellationToken ct = default);
    Task<LiaisonParent> CreateAsync(LiaisonParent entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
