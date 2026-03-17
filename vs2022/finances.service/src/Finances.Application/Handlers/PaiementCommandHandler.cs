using Finances.Application.Commands;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using MediatR;

namespace Finances.Application.Handlers;

public sealed class PaiementCommandHandler :
    IRequestHandler<CreatePaiementCommand, Paiement>,
    IRequestHandler<UpdatePaiementCommand, bool>,
    IRequestHandler<DeletePaiementCommand, bool>
{
    private readonly IPaiementService _service;

    public PaiementCommandHandler(IPaiementService service)
    {
        _service = service;
    }

    public async Task<Paiement> Handle(CreatePaiementCommand request, CancellationToken ct)
    {
        var entity = new Paiement
        {
            TypeId = request.TypeId,
            FactureId = request.FactureId,
            MontantPaye = request.MontantPaye,
            DatePaiement = request.DatePaiement,
            MoyenPaiement = request.MoyenPaiement,
            ReferenceMobileMoney = request.ReferenceMobileMoney,
            StatutPaiement = request.StatutPaiement,
            CaissierId = request.CaissierId,
            NumeroRecu = request.NumeroRecu,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdatePaiementCommand request, CancellationToken ct)
    {
        var entity = new Paiement
        {
            Id = request.Id,
            TypeId = request.TypeId,
            FactureId = request.FactureId,
            MontantPaye = request.MontantPaye,
            DatePaiement = request.DatePaiement,
            MoyenPaiement = request.MoyenPaiement,
            ReferenceMobileMoney = request.ReferenceMobileMoney,
            StatutPaiement = request.StatutPaiement,
            CaissierId = request.CaissierId,
            NumeroRecu = request.NumeroRecu,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeletePaiementCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
