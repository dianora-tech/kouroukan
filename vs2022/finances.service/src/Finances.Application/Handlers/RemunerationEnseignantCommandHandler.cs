using Finances.Application.Commands;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using MediatR;

namespace Finances.Application.Handlers;

public sealed class RemunerationEnseignantCommandHandler :
    IRequestHandler<CreateRemunerationEnseignantCommand, RemunerationEnseignant>,
    IRequestHandler<UpdateRemunerationEnseignantCommand, bool>,
    IRequestHandler<DeleteRemunerationEnseignantCommand, bool>
{
    private readonly IRemunerationEnseignantService _service;

    public RemunerationEnseignantCommandHandler(IRemunerationEnseignantService service)
    {
        _service = service;
    }

    public async Task<RemunerationEnseignant> Handle(CreateRemunerationEnseignantCommand request, CancellationToken ct)
    {
        var entity = new RemunerationEnseignant
        {
            EnseignantId = request.EnseignantId,
            Mois = request.Mois,
            Annee = request.Annee,
            ModeRemuneration = request.ModeRemuneration,
            MontantForfait = request.MontantForfait,
            NombreHeures = request.NombreHeures,
            TauxHoraire = request.TauxHoraire,
            StatutPaiement = request.StatutPaiement,
            DateValidation = request.DateValidation,
            ValidateurId = request.ValidateurId,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateRemunerationEnseignantCommand request, CancellationToken ct)
    {
        var entity = new RemunerationEnseignant
        {
            Id = request.Id,
            EnseignantId = request.EnseignantId,
            Mois = request.Mois,
            Annee = request.Annee,
            ModeRemuneration = request.ModeRemuneration,
            MontantForfait = request.MontantForfait,
            NombreHeures = request.NombreHeures,
            TauxHoraire = request.TauxHoraire,
            StatutPaiement = request.StatutPaiement,
            DateValidation = request.DateValidation,
            ValidateurId = request.ValidateurId,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteRemunerationEnseignantCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
