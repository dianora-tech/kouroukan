using Finances.Application.Commands;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using MediatR;

namespace Finances.Application.Handlers;

public sealed class FactureCommandHandler :
    IRequestHandler<CreateFactureCommand, Facture>,
    IRequestHandler<UpdateFactureCommand, bool>,
    IRequestHandler<DeleteFactureCommand, bool>
{
    private readonly IFactureService _service;

    public FactureCommandHandler(IFactureService service)
    {
        _service = service;
    }

    public async Task<Facture> Handle(CreateFactureCommand request, CancellationToken ct)
    {
        var entity = new Facture
        {
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            ParentId = request.ParentId,
            AnneeScolaireId = request.AnneeScolaireId,
            MontantTotal = request.MontantTotal,
            MontantPaye = request.MontantPaye,
            DateEmission = request.DateEmission,
            DateEcheance = request.DateEcheance,
            StatutFacture = request.StatutFacture,
            NumeroFacture = request.NumeroFacture,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateFactureCommand request, CancellationToken ct)
    {
        var entity = new Facture
        {
            Id = request.Id,
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            ParentId = request.ParentId,
            AnneeScolaireId = request.AnneeScolaireId,
            MontantTotal = request.MontantTotal,
            MontantPaye = request.MontantPaye,
            DateEmission = request.DateEmission,
            DateEcheance = request.DateEcheance,
            StatutFacture = request.StatutFacture,
            NumeroFacture = request.NumeroFacture,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteFactureCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
