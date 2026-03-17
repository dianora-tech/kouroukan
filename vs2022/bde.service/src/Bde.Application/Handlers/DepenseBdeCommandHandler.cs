using Bde.Application.Commands;
using Bde.Domain.Ports.Input;
using MediatR;
using DepenseBdeEntity = Bde.Domain.Entities.DepenseBde;

namespace Bde.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les depenses BDE.
/// </summary>
public sealed class DepenseBdeCommandHandler :
    IRequestHandler<CreateDepenseBdeCommand, DepenseBdeEntity>,
    IRequestHandler<UpdateDepenseBdeCommand, bool>,
    IRequestHandler<DeleteDepenseBdeCommand, bool>
{
    private readonly IDepenseBdeService _service;

    public DepenseBdeCommandHandler(IDepenseBdeService service)
    {
        _service = service;
    }

    public async Task<DepenseBdeEntity> Handle(CreateDepenseBdeCommand request, CancellationToken ct)
    {
        var entity = new DepenseBdeEntity
        {
            TypeId = request.TypeId,
            Name = request.Name,
            Description = request.Description,
            AssociationId = request.AssociationId,
            Montant = request.Montant,
            Motif = request.Motif,
            Categorie = request.Categorie,
            StatutValidation = request.StatutValidation,
            ValidateurId = request.ValidateurId,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateDepenseBdeCommand request, CancellationToken ct)
    {
        var entity = new DepenseBdeEntity
        {
            Id = request.Id,
            TypeId = request.TypeId,
            Name = request.Name,
            Description = request.Description,
            AssociationId = request.AssociationId,
            Montant = request.Montant,
            Motif = request.Motif,
            Categorie = request.Categorie,
            StatutValidation = request.StatutValidation,
            ValidateurId = request.ValidateurId,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteDepenseBdeCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
