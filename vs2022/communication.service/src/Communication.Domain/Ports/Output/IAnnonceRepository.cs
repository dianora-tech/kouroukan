using GnDapper.Models;
using Communication.Domain.Entities;

namespace Communication.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des annonces.
/// </summary>
public interface IAnnonceRepository
{
    Task<Annonce?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Annonce>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Annonce>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Annonce> AddAsync(Annonce entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Annonce entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}
