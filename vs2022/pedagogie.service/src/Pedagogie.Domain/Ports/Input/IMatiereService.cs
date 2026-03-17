using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des matieres.
/// </summary>
public interface IMatiereService
{
    Task<Matiere?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Matiere>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Matiere>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Matiere> CreateAsync(Matiere entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Matiere entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
