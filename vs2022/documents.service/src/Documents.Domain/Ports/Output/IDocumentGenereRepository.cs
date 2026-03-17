using GnDapper.Models;
using Documents.Domain.Entities;

namespace Documents.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des documents generes.
/// </summary>
public interface IDocumentGenereRepository
{
    Task<DocumentGenere?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DocumentGenere>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<DocumentGenere>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<DocumentGenere> AddAsync(DocumentGenere entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(DocumentGenere entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}
