using GnDapper.Models;
using Personnel.Domain.Entities;

namespace Personnel.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des demandes de conge.
/// </summary>
public interface IDemandeCongeService
{
    Task<DemandeConge?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DemandeConge>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<DemandeConge>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<DemandeConge> CreateAsync(DemandeConge entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(DemandeConge entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
