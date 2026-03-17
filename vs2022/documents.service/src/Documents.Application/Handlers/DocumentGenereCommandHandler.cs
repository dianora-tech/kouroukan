using Documents.Application.Commands;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using MediatR;

namespace Documents.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les documents generes.
/// </summary>
public sealed class DocumentGenereCommandHandler :
    IRequestHandler<CreateDocumentGenereCommand, DocumentGenere>,
    IRequestHandler<UpdateDocumentGenereCommand, bool>,
    IRequestHandler<DeleteDocumentGenereCommand, bool>
{
    private readonly IDocumentGenereService _service;

    public DocumentGenereCommandHandler(IDocumentGenereService service)
    {
        _service = service;
    }

    public async Task<DocumentGenere> Handle(CreateDocumentGenereCommand request, CancellationToken ct)
    {
        var entity = new DocumentGenere
        {
            TypeId = request.TypeId,
            ModeleDocumentId = request.ModeleDocumentId,
            EleveId = request.EleveId,
            EnseignantId = request.EnseignantId,
            DonneesJson = request.DonneesJson,
            DateGeneration = request.DateGeneration,
            StatutSignature = request.StatutSignature,
            CheminFichier = request.CheminFichier,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateDocumentGenereCommand request, CancellationToken ct)
    {
        var entity = new DocumentGenere
        {
            Id = request.Id,
            TypeId = request.TypeId,
            ModeleDocumentId = request.ModeleDocumentId,
            EleveId = request.EleveId,
            EnseignantId = request.EnseignantId,
            DonneesJson = request.DonneesJson,
            DateGeneration = request.DateGeneration,
            StatutSignature = request.StatutSignature,
            CheminFichier = request.CheminFichier,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteDocumentGenereCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
