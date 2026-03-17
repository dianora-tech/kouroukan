using MediatR;
using Support.Domain.Entities;

namespace Support.Application.Commands;

public sealed record CreerConversationIACommand(
    int UtilisateurId,
    int UserId) : IRequest<ConversationIA>;

public sealed record EnvoyerMessageIACommand(
    int ConversationId,
    string Question,
    int UserId) : IRequest<string>;

public sealed record GenererReponseIATicketCommand(
    int TicketId) : IRequest<string>;
