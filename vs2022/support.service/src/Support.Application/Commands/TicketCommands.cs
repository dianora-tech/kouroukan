using MediatR;
using Support.Domain.Entities;

namespace Support.Application.Commands;

public sealed record CreateTicketCommand(
    string Name,
    string? Description,
    int TypeId,
    int AuteurId,
    string Sujet,
    string Contenu,
    string Priorite,
    string CategorieTicket,
    string? ModuleConcerne,
    string? PieceJointeUrl,
    int UserId) : IRequest<Ticket>;

public sealed record UpdateTicketCommand(
    int Id,
    string Name,
    string? Description,
    int TypeId,
    int AuteurId,
    string Sujet,
    string Contenu,
    string Priorite,
    string StatutTicket,
    string CategorieTicket,
    string? ModuleConcerne,
    int? AssigneAId,
    DateTime? DateResolution,
    int? NoteSatisfaction,
    string? PieceJointeUrl,
    int UserId) : IRequest<bool>;

public sealed record DeleteTicketCommand(int Id) : IRequest<bool>;

public sealed record AddReponseTicketCommand(
    int TicketId,
    int AuteurId,
    string Contenu,
    bool EstReponseIA,
    bool EstInterne,
    int UserId) : IRequest<ReponseTicket>;
