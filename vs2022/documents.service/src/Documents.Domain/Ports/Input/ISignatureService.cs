using GnDapper.Models;
using Documents.Domain.Entities;

namespace Documents.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des signatures electroniques.
/// </summary>
public interface ISignatureService
{
    Task<Signature?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Signature>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Signature>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Signature> CreateAsync(Signature entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Signature entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
