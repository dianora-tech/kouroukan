using Presences.Application.Commands;
using Presences.Domain.Ports.Input;
using MediatR;
using BadgeageEntity = Presences.Domain.Entities.Badgeage;

namespace Presences.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les badgeages.
/// </summary>
public sealed class BadgeageCommandHandler :
    IRequestHandler<CreateBadgeageCommand, BadgeageEntity>,
    IRequestHandler<UpdateBadgeageCommand, bool>,
    IRequestHandler<DeleteBadgeageCommand, bool>
{
    private readonly IBadgeageService _service;

    public BadgeageCommandHandler(IBadgeageService service)
    {
        _service = service;
    }

    public async Task<BadgeageEntity> Handle(CreateBadgeageCommand request, CancellationToken ct)
    {
        var entity = new BadgeageEntity
        {
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            DateBadgeage = request.DateBadgeage,
            HeureBadgeage = request.HeureBadgeage,
            PointAcces = request.PointAcces,
            MethodeBadgeage = request.MethodeBadgeage,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateBadgeageCommand request, CancellationToken ct)
    {
        var entity = new BadgeageEntity
        {
            Id = request.Id,
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            DateBadgeage = request.DateBadgeage,
            HeureBadgeage = request.HeureBadgeage,
            PointAcces = request.PointAcces,
            MethodeBadgeage = request.MethodeBadgeage,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteBadgeageCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
