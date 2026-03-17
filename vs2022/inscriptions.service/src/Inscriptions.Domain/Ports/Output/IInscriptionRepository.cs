using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des inscriptions.
/// </summary>
public interface IInscriptionRepository
{
    Task<Inscription?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Inscription>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Inscription>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Inscription> AddAsync(Inscription entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Inscription entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}
