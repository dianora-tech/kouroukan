using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Commands;

/// <summary>
/// Commande de creation d'une notification.
/// </summary>
public sealed record CreateNotificationCommand(
    string Name,
    string? Description,
    int TypeId,
    string DestinatairesIds,
    string Contenu,
    string Canal,
    bool EstEnvoyee,
    DateTime? DateEnvoi,
    string? LienAction,
    int UserId) : IRequest<Notification>;
