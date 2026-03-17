using Finances.Application.Commands;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using MediatR;

namespace Finances.Application.Handlers;

public sealed class DepenseCommandHandler :
    IRequestHandler<CreateDepenseCommand, Depense>,
    IRequestHandler<UpdateDepenseCommand, bool>,
    IRequestHandler<DeleteDepenseCommand, bool>
{
    private readonly IDepenseService _service;

    public DepenseCommandHandler(IDepenseService service)
    {
        _service = service;
    }

    public async Task<Depense> Handle(CreateDepenseCommand request, CancellationToken ct)
    {
        var entity = new Depense
        {
            TypeId = request.TypeId,
            Montant = request.Montant,
            MotifDepense = request.MotifDepense,
            Categorie = request.Categorie,
            BeneficiaireNom = request.BeneficiaireNom,
            BeneficiaireTelephone = request.BeneficiaireTelephone,
            BeneficiaireNIF = request.BeneficiaireNIF,
            StatutDepense = request.StatutDepense,
            DateDemande = request.DateDemande,
            DateValidation = request.DateValidation,
            ValidateurId = request.ValidateurId,
            PieceJointeUrl = request.PieceJointeUrl,
            NumeroJustificatif = request.NumeroJustificatif,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateDepenseCommand request, CancellationToken ct)
    {
        var entity = new Depense
        {
            Id = request.Id,
            TypeId = request.TypeId,
            Montant = request.Montant,
            MotifDepense = request.MotifDepense,
            Categorie = request.Categorie,
            BeneficiaireNom = request.BeneficiaireNom,
            BeneficiaireTelephone = request.BeneficiaireTelephone,
            BeneficiaireNIF = request.BeneficiaireNIF,
            StatutDepense = request.StatutDepense,
            DateDemande = request.DateDemande,
            DateValidation = request.DateValidation,
            ValidateurId = request.ValidateurId,
            PieceJointeUrl = request.PieceJointeUrl,
            NumeroJustificatif = request.NumeroJustificatif,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteDepenseCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
