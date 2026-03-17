using GnDapper.Models;
using Documents.Application.Queries;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using MediatR;

namespace Documents.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les signatures.
/// </summary>
public sealed class SignatureQueryHandler :
    IRequestHandler<GetSignatureByIdQuery, Signature?>,
    IRequestHandler<GetAllSignaturesQuery, IReadOnlyList<Signature>>,
    IRequestHandler<GetPagedSignaturesQuery, PagedResult<Signature>>
{
    private readonly ISignatureService _service;

    public SignatureQueryHandler(ISignatureService service)
    {
        _service = service;
    }

    public async Task<Signature?> Handle(GetSignatureByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Signature>> Handle(GetAllSignaturesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Signature>> Handle(GetPagedSignaturesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
