using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Queries;

/// <summary>
/// Requete de recuperation d'un message par son identifiant.
/// </summary>
public sealed record GetMessageByIdQuery(int Id) : IRequest<Message?>;
