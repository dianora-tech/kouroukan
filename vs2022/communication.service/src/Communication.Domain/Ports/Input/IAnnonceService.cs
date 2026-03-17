using GnDapper.Models;
using Communication.Domain.Entities;

namespace Communication.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des annonces.
/// </summary>
public interface IAnnonceService
{
    Task<Annonce?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Annonce>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Annonce>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Annonce> CreateAsync(Annonce entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Annonce entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
