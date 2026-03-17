using GnDapper.Models;
using Evaluations.Application.Queries;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using MediatR;

namespace Evaluations.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les notes.
/// </summary>
public sealed class NoteQueryHandler :
    IRequestHandler<GetNoteByIdQuery, Note?>,
    IRequestHandler<GetAllNotesQuery, IReadOnlyList<Note>>,
    IRequestHandler<GetPagedNotesQuery, PagedResult<Note>>
{
    private readonly INoteService _service;

    public NoteQueryHandler(INoteService service)
    {
        _service = service;
    }

    public async Task<Note?> Handle(GetNoteByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Note>> Handle(GetAllNotesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Note>> Handle(GetPagedNotesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
