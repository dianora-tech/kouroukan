using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des matieres.
/// </summary>
public interface IMatiereRepository
{
    Task<Matiere?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Matiere>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Matiere>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Matiere> AddAsync(Matiere entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Matiere entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
    Task<TypeDto?> GetTypeByIdAsync(int id, CancellationToken ct = default);
    Task<TypeDto> AddTypeAsync(string name, string? description, CancellationToken ct = default);
    Task<bool> UpdateTypeAsync(int id, string name, string? description, CancellationToken ct = default);
    Task<bool> DeleteTypeAsync(int id, CancellationToken ct = default);
}
