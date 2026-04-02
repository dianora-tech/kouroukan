using Inscriptions.Application.Commands;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les transferts.
/// </summary>
public sealed class TransfertCommandHandler :
    IRequestHandler<CreateTransfertCommand, Transfert>,
    IRequestHandler<AcceptTransfertCommand, bool>,
    IRequestHandler<RejectTransfertCommand, bool>,
    IRequestHandler<CompleteTransfertCommand, bool>
{
    private readonly ITransfertService _service;

    public TransfertCommandHandler(ITransfertService service)
    {
        _service = service;
    }

    public async Task<Transfert> Handle(CreateTransfertCommand request, CancellationToken ct)
    {
        var entity = new Transfert
        {
            EleveId = request.EleveId,
            CompanyOrigineId = request.CompanyOrigineId,
            CompanyCibleId = request.CompanyCibleId,
            Motif = request.Motif,
            Documents = request.Documents
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(AcceptTransfertCommand request, CancellationToken ct)
    {
        return await _service.AcceptAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(RejectTransfertCommand request, CancellationToken ct)
    {
        return await _service.RejectAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(CompleteTransfertCommand request, CancellationToken ct)
    {
        return await _service.CompleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
