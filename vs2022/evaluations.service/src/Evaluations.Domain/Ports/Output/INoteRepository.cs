using GnDapper.Models;
using Evaluations.Domain.Entities;

namespace Evaluations.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des notes.
/// </summary>
public interface INoteRepository
{
    Task<Note?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Note>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Note> AddAsync(Note entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Note entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Note>> GetByEvaluationIdAsync(int evaluationId, CancellationToken ct = default);
    Task<IReadOnlyList<Note>> GetByEleveIdAsync(int eleveId, CancellationToken ct = default);
}
