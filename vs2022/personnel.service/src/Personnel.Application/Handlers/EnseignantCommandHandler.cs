using Personnel.Application.Commands;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using MediatR;

namespace Personnel.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les enseignants.
/// </summary>
public sealed class EnseignantCommandHandler :
    IRequestHandler<CreateEnseignantCommand, Enseignant>,
    IRequestHandler<UpdateEnseignantCommand, bool>,
    IRequestHandler<DeleteEnseignantCommand, bool>
{
    private readonly IEnseignantService _service;

    public EnseignantCommandHandler(IEnseignantService service)
    {
        _service = service;
    }

    public async Task<Enseignant> Handle(CreateEnseignantCommand request, CancellationToken ct)
    {
        var entity = new Enseignant
        {
            Name = request.Name,
            Description = request.Description,
            Matricule = request.Matricule,
            Specialite = request.Specialite,
            DateEmbauche = request.DateEmbauche,
            ModeRemuneration = request.ModeRemuneration,
            MontantForfait = request.MontantForfait,
            Telephone = request.Telephone,
            Email = request.Email,
            StatutEnseignant = request.StatutEnseignant,
            SoldeCongesAnnuel = request.SoldeCongesAnnuel,
            TypeId = request.TypeId,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateEnseignantCommand request, CancellationToken ct)
    {
        var entity = new Enseignant
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            Matricule = request.Matricule,
            Specialite = request.Specialite,
            DateEmbauche = request.DateEmbauche,
            ModeRemuneration = request.ModeRemuneration,
            MontantForfait = request.MontantForfait,
            Telephone = request.Telephone,
            Email = request.Email,
            StatutEnseignant = request.StatutEnseignant,
            SoldeCongesAnnuel = request.SoldeCongesAnnuel,
            TypeId = request.TypeId,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteEnseignantCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
