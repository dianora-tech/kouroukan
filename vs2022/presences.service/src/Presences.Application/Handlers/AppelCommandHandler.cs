using Presences.Application.Commands;
using Presences.Domain.Ports.Input;
using MediatR;
using AppelEntity = Presences.Domain.Entities.Appel;

namespace Presences.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les appels.
/// </summary>
public sealed class AppelCommandHandler :
    IRequestHandler<CreateAppelCommand, AppelEntity>,
    IRequestHandler<UpdateAppelCommand, bool>,
    IRequestHandler<DeleteAppelCommand, bool>
{
    private readonly IAppelService _service;

    public AppelCommandHandler(IAppelService service)
    {
        _service = service;
    }

    public async Task<AppelEntity> Handle(CreateAppelCommand request, CancellationToken ct)
    {
        var entity = new AppelEntity
        {
            ClasseId = request.ClasseId,
            EnseignantId = request.EnseignantId,
            SeanceId = request.SeanceId,
            DateAppel = request.DateAppel,
            HeureAppel = request.HeureAppel,
            EstCloture = request.EstCloture,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateAppelCommand request, CancellationToken ct)
    {
        var entity = new AppelEntity
        {
            Id = request.Id,
            ClasseId = request.ClasseId,
            EnseignantId = request.EnseignantId,
            SeanceId = request.SeanceId,
            DateAppel = request.DateAppel,
            HeureAppel = request.HeureAppel,
            EstCloture = request.EstCloture,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteAppelCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
