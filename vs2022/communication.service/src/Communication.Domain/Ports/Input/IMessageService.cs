using GnDapper.Models;
using Communication.Domain.Entities;

namespace Communication.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des messages.
/// </summary>
public interface IMessageService
{
    Task<Message?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Message>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Message>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Message> CreateAsync(Message entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Message entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
