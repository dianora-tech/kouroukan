using GnDapper.Models;
using Communication.Application.Queries;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using MediatR;

namespace Communication.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les messages.
/// </summary>
public sealed class MessageQueryHandler :
    IRequestHandler<GetMessageByIdQuery, Message?>,
    IRequestHandler<GetAllMessagesQuery, IReadOnlyList<Message>>,
    IRequestHandler<GetPagedMessagesQuery, PagedResult<Message>>
{
    private readonly IMessageService _service;

    public MessageQueryHandler(IMessageService service)
    {
        _service = service;
    }

    public async Task<Message?> Handle(GetMessageByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Message>> Handle(GetAllMessagesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Message>> Handle(GetPagedMessagesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
