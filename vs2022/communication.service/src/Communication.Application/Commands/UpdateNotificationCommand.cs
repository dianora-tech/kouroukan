using MediatR;

namespace Communication.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une notification.
/// </summary>
public sealed record UpdateNotificationCommand(
    int Id,
    string Name,
    string? Description,
    int TypeId,
    string DestinatairesIds,
    string Contenu,
    string Canal,
    bool EstEnvoyee,
    DateTime? DateEnvoi,
    string? LienAction,
    int UserId) : IRequest<bool>;
