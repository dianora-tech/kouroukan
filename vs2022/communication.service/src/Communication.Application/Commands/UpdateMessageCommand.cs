using MediatR;

namespace Communication.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un message.
/// </summary>
public sealed record UpdateMessageCommand(
    int Id,
    string Name,
    string? Description,
    int TypeId,
    int ExpediteurId,
    int? DestinataireId,
    string Sujet,
    string Contenu,
    bool EstLu,
    DateTime? DateLecture,
    string? GroupeDestinataire,
    int UserId) : IRequest<bool>;
