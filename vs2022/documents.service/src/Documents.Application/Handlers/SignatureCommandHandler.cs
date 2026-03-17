using Documents.Application.Commands;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using MediatR;

namespace Documents.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les signatures.
/// </summary>
public sealed class SignatureCommandHandler :
    IRequestHandler<CreateSignatureCommand, Signature>,
    IRequestHandler<UpdateSignatureCommand, bool>,
    IRequestHandler<DeleteSignatureCommand, bool>
{
    private readonly ISignatureService _service;

    public SignatureCommandHandler(ISignatureService service)
    {
        _service = service;
    }

    public async Task<Signature> Handle(CreateSignatureCommand request, CancellationToken ct)
    {
        var entity = new Signature
        {
            TypeId = request.TypeId,
            DocumentGenereId = request.DocumentGenereId,
            SignataireId = request.SignataireId,
            OrdreSignature = request.OrdreSignature,
            DateSignature = request.DateSignature,
            StatutSignature = request.StatutSignature,
            NiveauSignature = request.NiveauSignature,
            MotifRefus = request.MotifRefus,
            EstValidee = request.EstValidee,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateSignatureCommand request, CancellationToken ct)
    {
        var entity = new Signature
        {
            Id = request.Id,
            TypeId = request.TypeId,
            DocumentGenereId = request.DocumentGenereId,
            SignataireId = request.SignataireId,
            OrdreSignature = request.OrdreSignature,
            DateSignature = request.DateSignature,
            StatutSignature = request.StatutSignature,
            NiveauSignature = request.NiveauSignature,
            MotifRefus = request.MotifRefus,
            EstValidee = request.EstValidee,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteSignatureCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
