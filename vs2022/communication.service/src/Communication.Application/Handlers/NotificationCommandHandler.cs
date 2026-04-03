using Communication.Application.Commands;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using MediatR;

namespace Communication.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les notifications.
/// </summary>
public sealed class NotificationCommandHandler :
    IRequestHandler<CreateNotificationCommand, Notification>,
    IRequestHandler<UpdateNotificationCommand, bool>,
    IRequestHandler<DeleteNotificationCommand, bool>
{
    private readonly INotificationService _service;

    public NotificationCommandHandler(INotificationService service)
    {
        _service = service;
    }

    public async Task<Notification> Handle(CreateNotificationCommand request, CancellationToken ct)
    {
        var entity = new Notification
        {
            TypeId = request.TypeId,
            DestinatairesIds = request.DestinatairesIds,
            Contenu = request.Contenu,
            Canal = request.Canal,
            EstEnvoyee = request.EstEnvoyee,
            DateEnvoi = request.DateEnvoi,
            LienAction = request.LienAction,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateNotificationCommand request, CancellationToken ct)
    {
        var entity = new Notification
        {
            Id = request.Id,
            TypeId = request.TypeId,
            DestinatairesIds = request.DestinatairesIds,
            Contenu = request.Contenu,
            Canal = request.Canal,
            EstEnvoyee = request.EstEnvoyee,
            DateEnvoi = request.DateEnvoi,
            LienAction = request.LienAction,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
