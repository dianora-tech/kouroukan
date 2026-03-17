using Evaluations.Application.Commands;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using MediatR;

namespace Evaluations.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les bulletins.
/// </summary>
public sealed class BulletinCommandHandler :
    IRequestHandler<CreateBulletinCommand, Bulletin>,
    IRequestHandler<UpdateBulletinCommand, bool>,
    IRequestHandler<DeleteBulletinCommand, bool>
{
    private readonly IBulletinService _service;

    public BulletinCommandHandler(IBulletinService service)
    {
        _service = service;
    }

    public async Task<Bulletin> Handle(CreateBulletinCommand request, CancellationToken ct)
    {
        var entity = new Bulletin
        {
            Name = request.Name,
            Description = request.Description,
            EleveId = request.EleveId,
            ClasseId = request.ClasseId,
            Trimestre = request.Trimestre,
            AnneeScolaireId = request.AnneeScolaireId,
            MoyenneGenerale = request.MoyenneGenerale,
            Rang = request.Rang,
            Appreciation = request.Appreciation,
            EstPublie = request.EstPublie,
            DateGeneration = request.DateGeneration,
            CheminFichierPdf = request.CheminFichierPdf,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateBulletinCommand request, CancellationToken ct)
    {
        var entity = new Bulletin
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            EleveId = request.EleveId,
            ClasseId = request.ClasseId,
            Trimestre = request.Trimestre,
            AnneeScolaireId = request.AnneeScolaireId,
            MoyenneGenerale = request.MoyenneGenerale,
            Rang = request.Rang,
            Appreciation = request.Appreciation,
            EstPublie = request.EstPublie,
            DateGeneration = request.DateGeneration,
            CheminFichierPdf = request.CheminFichierPdf,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteBulletinCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
