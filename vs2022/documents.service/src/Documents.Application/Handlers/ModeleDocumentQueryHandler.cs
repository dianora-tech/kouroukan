using GnDapper.Models;
using Documents.Application.Queries;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using MediatR;

namespace Documents.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les modeles de documents.
/// </summary>
public sealed class ModeleDocumentQueryHandler :
    IRequestHandler<GetModeleDocumentByIdQuery, ModeleDocument?>,
    IRequestHandler<GetAllModeleDocumentsQuery, IReadOnlyList<ModeleDocument>>,
    IRequestHandler<GetPagedModeleDocumentsQuery, PagedResult<ModeleDocument>>
{
    private readonly IModeleDocumentService _service;

    public ModeleDocumentQueryHandler(IModeleDocumentService service)
    {
        _service = service;
    }

    public async Task<ModeleDocument?> Handle(GetModeleDocumentByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<ModeleDocument>> Handle(GetAllModeleDocumentsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<ModeleDocument>> Handle(GetPagedModeleDocumentsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
