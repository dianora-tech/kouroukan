using Bde.Application.Commands;
using Bde.Domain.Ports.Input;
using MediatR;
using MembreBdeEntity = Bde.Domain.Entities.MembreBde;

namespace Bde.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les membres BDE.
/// </summary>
public sealed class MembreBdeCommandHandler :
    IRequestHandler<CreateMembreBdeCommand, MembreBdeEntity>,
    IRequestHandler<UpdateMembreBdeCommand, bool>,
    IRequestHandler<DeleteMembreBdeCommand, bool>
{
    private readonly IMembreBdeService _service;

    public MembreBdeCommandHandler(IMembreBdeService service)
    {
        _service = service;
    }

    public async Task<MembreBdeEntity> Handle(CreateMembreBdeCommand request, CancellationToken ct)
    {
        var entity = new MembreBdeEntity
        {
            Name = request.Name,
            Description = request.Description,
            AssociationId = request.AssociationId,
            EleveId = request.EleveId,
            RoleBde = request.RoleBde,
            DateAdhesion = request.DateAdhesion,
            MontantCotisation = request.MontantCotisation,
            EstActif = request.EstActif,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateMembreBdeCommand request, CancellationToken ct)
    {
        var entity = new MembreBdeEntity
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            AssociationId = request.AssociationId,
            EleveId = request.EleveId,
            RoleBde = request.RoleBde,
            DateAdhesion = request.DateAdhesion,
            MontantCotisation = request.MontantCotisation,
            EstActif = request.EstActif,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteMembreBdeCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
