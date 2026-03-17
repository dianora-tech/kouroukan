using GnDapper.Models;
using Documents.Domain.Entities;

namespace Documents.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des documents generes.
/// </summary>
public interface IDocumentGenereService
{
    Task<DocumentGenere?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DocumentGenere>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<DocumentGenere>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<DocumentGenere> CreateAsync(DocumentGenere entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(DocumentGenere entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
