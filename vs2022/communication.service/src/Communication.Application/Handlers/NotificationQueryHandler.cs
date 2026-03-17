using GnDapper.Models;
using Communication.Application.Queries;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using MediatR;

namespace Communication.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les notifications.
/// </summary>
public sealed class NotificationQueryHandler :
    IRequestHandler<GetNotificationByIdQuery, Notification?>,
    IRequestHandler<GetAllNotificationsQuery, IReadOnlyList<Notification>>,
    IRequestHandler<GetPagedNotificationsQuery, PagedResult<Notification>>
{
    private readonly INotificationService _service;

    public NotificationQueryHandler(INotificationService service)
    {
        _service = service;
    }

    public async Task<Notification?> Handle(GetNotificationByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Notification>> Handle(GetAllNotificationsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Notification>> Handle(GetPagedNotificationsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
