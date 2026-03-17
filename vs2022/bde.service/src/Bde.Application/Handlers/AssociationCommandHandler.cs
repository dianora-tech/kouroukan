using Bde.Application.Commands;
using Bde.Domain.Ports.Input;
using MediatR;
using AssociationEntity = Bde.Domain.Entities.Association;

namespace Bde.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les associations.
/// </summary>
public sealed class AssociationCommandHandler :
    IRequestHandler<CreateAssociationCommand, AssociationEntity>,
    IRequestHandler<UpdateAssociationCommand, bool>,
    IRequestHandler<DeleteAssociationCommand, bool>
{
    private readonly IAssociationService _service;

    public AssociationCommandHandler(IAssociationService service)
    {
        _service = service;
    }

    public async Task<AssociationEntity> Handle(CreateAssociationCommand request, CancellationToken ct)
    {
        var entity = new AssociationEntity
        {
            TypeId = request.TypeId,
            Name = request.Name,
            Description = request.Description,
            Sigle = request.Sigle,
            AnneeScolaire = request.AnneeScolaire,
            Statut = request.Statut,
            BudgetAnnuel = request.BudgetAnnuel,
            SuperviseurId = request.SuperviseurId,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateAssociationCommand request, CancellationToken ct)
    {
        var entity = new AssociationEntity
        {
            Id = request.Id,
            TypeId = request.TypeId,
            Name = request.Name,
            Description = request.Description,
            Sigle = request.Sigle,
            AnneeScolaire = request.AnneeScolaire,
            Statut = request.Statut,
            BudgetAnnuel = request.BudgetAnnuel,
            SuperviseurId = request.SuperviseurId,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteAssociationCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
