using GnDapper.Models;
using Communication.Domain.Entities;

namespace Communication.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des notifications.
/// </summary>
public interface INotificationService
{
    Task<Notification?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Notification>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Notification> CreateAsync(Notification entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Notification entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
