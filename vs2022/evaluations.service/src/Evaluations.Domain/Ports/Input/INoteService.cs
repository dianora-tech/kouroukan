using GnDapper.Models;
using Evaluations.Domain.Entities;

namespace Evaluations.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des notes.
/// </summary>
public interface INoteService
{
    Task<Note?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Note>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Note> CreateAsync(Note entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Note entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
