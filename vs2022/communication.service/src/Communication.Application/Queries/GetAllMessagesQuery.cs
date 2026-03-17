using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les messages.
/// </summary>
public sealed record GetAllMessagesQuery() : IRequest<IReadOnlyList<Message>>;
