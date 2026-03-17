using MediatR;
using Support.Application.Commands;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;

namespace Support.Application.Handlers;

/// <summary>
/// Handler pour les commandes de tickets.
/// </summary>
public sealed class TicketCommandHandler :
    IRequestHandler<CreateTicketCommand, Ticket>,
    IRequestHandler<UpdateTicketCommand, bool>,
    IRequestHandler<DeleteTicketCommand, bool>,
    IRequestHandler<AddReponseTicketCommand, ReponseTicket>
{
    private readonly ITicketService _service;

    public TicketCommandHandler(ITicketService service) => _service = service;

    public async Task<Ticket> Handle(CreateTicketCommand request, CancellationToken ct)
    {
        var entity = new Ticket
        {
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            AuteurId = request.AuteurId,
            Sujet = request.Sujet,
            Contenu = request.Contenu,
            Priorite = request.Priorite,
            CategorieTicket = request.CategorieTicket,
            ModuleConcerne = request.ModuleConcerne,
            PieceJointeUrl = request.PieceJointeUrl,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct);
    }

    public async Task<bool> Handle(UpdateTicketCommand request, CancellationToken ct)
    {
        var entity = new Ticket
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            AuteurId = request.AuteurId,
            Sujet = request.Sujet,
            Contenu = request.Contenu,
            Priorite = request.Priorite,
            StatutTicket = request.StatutTicket,
            CategorieTicket = request.CategorieTicket,
            ModuleConcerne = request.ModuleConcerne,
            AssigneAId = request.AssigneAId,
            DateResolution = request.DateResolution,
            NoteSatisfaction = request.NoteSatisfaction,
            PieceJointeUrl = request.PieceJointeUrl,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct);
    }

    public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct);
    }

    public async Task<ReponseTicket> Handle(AddReponseTicketCommand request, CancellationToken ct)
    {
        var reponse = new ReponseTicket
        {
            TicketId = request.TicketId,
            AuteurId = request.AuteurId,
            Contenu = request.Contenu,
            EstReponseIA = request.EstReponseIA,
            EstInterne = request.EstInterne,
            UserId = request.UserId
        };
        return await _service.AddReponseAsync(reponse, ct);
    }
}
