using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Commands;

/// <summary>
/// Commande de creation d'un message.
/// </summary>
public sealed record CreateMessageCommand(
    int TypeId,
    int ExpediteurId,
    int? DestinataireId,
    string Sujet,
    string Contenu,
    bool EstLu,
    DateTime? DateLecture,
    string? GroupeDestinataire,
    int UserId) : IRequest<Message>;
