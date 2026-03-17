using GnDapper.Entities;
using GnDapper.Models;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour l'acces aux donnees des souscriptions.
/// </summary>
public interface ISouscriptionRepository
{
    Task<Souscription?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Souscription>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Souscription>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Souscription> AddAsync(Souscription entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Souscription entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
