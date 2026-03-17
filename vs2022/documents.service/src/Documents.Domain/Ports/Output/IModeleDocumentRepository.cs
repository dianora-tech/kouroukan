using GnDapper.Models;
using Documents.Domain.Entities;

namespace Documents.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des modeles de documents.
/// </summary>
public interface IModeleDocumentRepository
{
    Task<ModeleDocument?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ModeleDocument>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<ModeleDocument>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<ModeleDocument> AddAsync(ModeleDocument entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(ModeleDocument entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}

/// <summary>
/// DTO generique pour les tables de types (id, name, description).
/// </summary>
public sealed record TypeDto(int Id, string Name, string? Description);
