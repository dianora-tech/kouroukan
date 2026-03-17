using GnDapper.Models;
using Communication.Domain.Entities;

namespace Communication.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des notifications.
/// </summary>
public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Notification>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Notification> AddAsync(Notification entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Notification entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}
