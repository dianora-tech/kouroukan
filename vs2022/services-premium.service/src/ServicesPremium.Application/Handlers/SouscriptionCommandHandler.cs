using MediatR;
using ServicesPremium.Application.Commands;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Input;

namespace ServicesPremium.Application.Handlers;

/// <summary>
/// Handler pour les commandes CRUD de Souscription.
/// </summary>
public sealed class SouscriptionCommandHandler :
    IRequestHandler<CreateSouscriptionCommand, Souscription>,
    IRequestHandler<UpdateSouscriptionCommand, bool>,
    IRequestHandler<DeleteSouscriptionCommand, bool>
{
    private readonly ISouscriptionService _service;

    public SouscriptionCommandHandler(ISouscriptionService service)
    {
        _service = service;
    }

    /// <summary>Cree une nouvelle souscription.</summary>
    public async Task<Souscription> Handle(CreateSouscriptionCommand request, CancellationToken ct)
    {
        var entity = new Souscription
        {
            Name = request.Name,
            Description = request.Description,
            ServiceParentId = request.ServiceParentId,
            ParentId = request.ParentId,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            StatutSouscription = request.StatutSouscription,
            MontantPaye = request.MontantPaye,
            RenouvellementAuto = request.RenouvellementAuto,
            DateProchainRenouvellement = request.DateProchainRenouvellement,
            UserId = request.UserId
        };

        return await _service.CreateAsync(entity, ct);
    }

    /// <summary>Met a jour une souscription existante.</summary>
    public async Task<bool> Handle(UpdateSouscriptionCommand request, CancellationToken ct)
    {
        var entity = new Souscription
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            ServiceParentId = request.ServiceParentId,
            ParentId = request.ParentId,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            StatutSouscription = request.StatutSouscription,
            MontantPaye = request.MontantPaye,
            RenouvellementAuto = request.RenouvellementAuto,
            DateProchainRenouvellement = request.DateProchainRenouvellement,
            UserId = request.UserId
        };

        return await _service.UpdateAsync(entity, ct);
    }

    /// <summary>Supprime une souscription (soft delete).</summary>
    public async Task<bool> Handle(DeleteSouscriptionCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct);
    }
}
