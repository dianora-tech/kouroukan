using Bde.Application.Commands;
using Bde.Domain.Ports.Input;
using MediatR;
using EvenementEntity = Bde.Domain.Entities.Evenement;

namespace Bde.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les evenements.
/// </summary>
public sealed class EvenementCommandHandler :
    IRequestHandler<CreateEvenementCommand, EvenementEntity>,
    IRequestHandler<UpdateEvenementCommand, bool>,
    IRequestHandler<DeleteEvenementCommand, bool>
{
    private readonly IEvenementService _service;

    public EvenementCommandHandler(IEvenementService service)
    {
        _service = service;
    }

    public async Task<EvenementEntity> Handle(CreateEvenementCommand request, CancellationToken ct)
    {
        var entity = new EvenementEntity
        {
            TypeId = request.TypeId,
            Name = request.Name,
            Description = request.Description,
            AssociationId = request.AssociationId,
            DateEvenement = request.DateEvenement,
            Lieu = request.Lieu,
            Capacite = request.Capacite,
            TarifEntree = request.TarifEntree,
            NombreInscrits = request.NombreInscrits,
            StatutEvenement = request.StatutEvenement,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateEvenementCommand request, CancellationToken ct)
    {
        var entity = new EvenementEntity
        {
            Id = request.Id,
            TypeId = request.TypeId,
            Name = request.Name,
            Description = request.Description,
            AssociationId = request.AssociationId,
            DateEvenement = request.DateEvenement,
            Lieu = request.Lieu,
            Capacite = request.Capacite,
            TarifEntree = request.TarifEntree,
            NombreInscrits = request.NombreInscrits,
            StatutEvenement = request.StatutEvenement,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteEvenementCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
