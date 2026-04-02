using Finances.Application.Commands;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using MediatR;

namespace Finances.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les moyens de paiement.
/// </summary>
public sealed class MoyenPaiementCommandHandler :
    IRequestHandler<CreateMoyenPaiementCommand, MoyenPaiement>,
    IRequestHandler<UpdateMoyenPaiementCommand, bool>,
    IRequestHandler<DeleteMoyenPaiementCommand, bool>
{
    private readonly IMoyenPaiementService _service;

    public MoyenPaiementCommandHandler(IMoyenPaiementService service)
    {
        _service = service;
    }

    public async Task<MoyenPaiement> Handle(CreateMoyenPaiementCommand request, CancellationToken ct)
    {
        var entity = new MoyenPaiement
        {
            CompanyId = request.CompanyId,
            Operateur = request.Operateur,
            NumeroCompte = request.NumeroCompte,
            CodeMarchand = request.CodeMarchand,
            Libelle = request.Libelle
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateMoyenPaiementCommand request, CancellationToken ct)
    {
        var entity = new MoyenPaiement
        {
            Id = request.Id,
            CompanyId = request.CompanyId,
            Operateur = request.Operateur,
            NumeroCompte = request.NumeroCompte,
            CodeMarchand = request.CodeMarchand,
            Libelle = request.Libelle,
            EstActif = request.EstActif
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteMoyenPaiementCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
