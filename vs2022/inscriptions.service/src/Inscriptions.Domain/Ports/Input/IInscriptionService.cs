using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des inscriptions.
/// </summary>
public interface IInscriptionService
{
    Task<Inscription?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Inscription>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Inscription>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Inscription> CreateAsync(Inscription entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Inscription entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
