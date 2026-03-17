using GnDapper.Models;
using Evaluations.Domain.Entities;

namespace Evaluations.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des evaluations.
/// </summary>
public interface IEvaluationService
{
    Task<Evaluation?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Evaluation>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Evaluation>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Evaluation> CreateAsync(Evaluation entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Evaluation entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
