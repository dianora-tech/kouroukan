using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des annees scolaires.
/// </summary>
public interface IAnneeScolaireRepository
{
    Task<AnneeScolaire?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<AnneeScolaire>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<AnneeScolaire>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<AnneeScolaire> AddAsync(AnneeScolaire entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(AnneeScolaire entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
