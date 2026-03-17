using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des annees scolaires.
/// </summary>
public interface IAnneeScolaireService
{
    Task<AnneeScolaire?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<AnneeScolaire>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<AnneeScolaire>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<AnneeScolaire> CreateAsync(AnneeScolaire entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(AnneeScolaire entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
