using MediatR;
using Support.Domain.Entities;

namespace Support.Application.Queries;

public sealed record GetMessagesConversationIAQuery(int ConversationId) : IRequest<IReadOnlyList<MessageIA>>;
