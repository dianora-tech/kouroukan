using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des liaisons parent.
/// </summary>
public interface ILiaisonParentRepository
{
    Task<LiaisonParent?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<LiaisonParent>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<LiaisonParent>> GetPagedAsync(int page, int pageSize, int? parentUserId, int? companyId, string? orderBy, CancellationToken ct = default);
    Task<LiaisonParent> AddAsync(LiaisonParent entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
