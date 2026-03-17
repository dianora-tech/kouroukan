using GnDapper.Models;
using Documents.Application.Queries;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using MediatR;

namespace Documents.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les documents generes.
/// </summary>
public sealed class DocumentGenereQueryHandler :
    IRequestHandler<GetDocumentGenereByIdQuery, DocumentGenere?>,
    IRequestHandler<GetAllDocumentGeneresQuery, IReadOnlyList<DocumentGenere>>,
    IRequestHandler<GetPagedDocumentGeneresQuery, PagedResult<DocumentGenere>>
{
    private readonly IDocumentGenereService _service;

    public DocumentGenereQueryHandler(IDocumentGenereService service)
    {
        _service = service;
    }

    public async Task<DocumentGenere?> Handle(GetDocumentGenereByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DocumentGenere>> Handle(GetAllDocumentGeneresQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<DocumentGenere>> Handle(GetPagedDocumentGeneresQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
