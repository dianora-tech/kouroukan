using Inscriptions.Application.Commands;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les annees scolaires.
/// </summary>
public sealed class AnneeScolaireCommandHandler :
    IRequestHandler<CreateAnneeScolaireCommand, AnneeScolaire>,
    IRequestHandler<UpdateAnneeScolaireCommand, bool>,
    IRequestHandler<DeleteAnneeScolaireCommand, bool>
{
    private readonly IAnneeScolaireService _service;

    public AnneeScolaireCommandHandler(IAnneeScolaireService service)
    {
        _service = service;
    }

    public async Task<AnneeScolaire> Handle(CreateAnneeScolaireCommand request, CancellationToken ct)
    {
        var entity = new AnneeScolaire
        {
            Libelle = request.Libelle,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            EstActive = request.EstActive,
            Code = request.Code,
            Description = request.Description,
            Statut = request.Statut,
            DateRentree = request.DateRentree,
            NombrePeriodes = request.NombrePeriodes,
            TypePeriode = request.TypePeriode
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateAnneeScolaireCommand request, CancellationToken ct)
    {
        var entity = new AnneeScolaire
        {
            Id = request.Id,
            Libelle = request.Libelle,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            EstActive = request.EstActive,
            Code = request.Code,
            Description = request.Description,
            Statut = request.Statut,
            DateRentree = request.DateRentree,
            NombrePeriodes = request.NombrePeriodes,
            TypePeriode = request.TypePeriode
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteAnneeScolaireCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
