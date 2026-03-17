using GnDapper.Models;
using Evaluations.Domain.Entities;

namespace Evaluations.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des evaluations.
/// </summary>
public interface IEvaluationRepository
{
    Task<Evaluation?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Evaluation>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Evaluation>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Evaluation> AddAsync(Evaluation entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Evaluation entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}

/// <summary>
/// DTO generique pour les tables de types (id, name, description).
/// </summary>
public sealed record TypeDto(int Id, string Name, string? Description);
