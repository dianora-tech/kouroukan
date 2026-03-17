using MediatR;
using Support.Application.Commands;
using Support.Application.Queries;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;

namespace Support.Application.Handlers;

/// <summary>
/// Handler pour les commandes et requetes de conversations IA.
/// </summary>
public sealed class ConversationIAHandler :
    IRequestHandler<CreerConversationIACommand, ConversationIA>,
    IRequestHandler<EnvoyerMessageIACommand, string>,
    IRequestHandler<GenererReponseIATicketCommand, string>,
    IRequestHandler<GetMessagesConversationIAQuery, IReadOnlyList<MessageIA>>
{
    private readonly IAideIAService _service;

    public ConversationIAHandler(IAideIAService service) => _service = service;

    public async Task<ConversationIA> Handle(CreerConversationIACommand request, CancellationToken ct) =>
        await _service.CreerConversationAsync(request.UtilisateurId, request.UserId, ct);

    public async Task<string> Handle(EnvoyerMessageIACommand request, CancellationToken ct) =>
        await _service.GenererReponseAsync(request.ConversationId, request.Question, ct);

    public async Task<string> Handle(GenererReponseIATicketCommand request, CancellationToken ct) =>
        await _service.SuggererReponseTicketAsync(request.TicketId, ct);

    public async Task<IReadOnlyList<MessageIA>> Handle(GetMessagesConversationIAQuery request, CancellationToken ct) =>
        await _service.GetMessagesAsync(request.ConversationId, ct);
}
