using Communication.Application.Commands;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using MediatR;

namespace Communication.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les messages.
/// </summary>
public sealed class MessageCommandHandler :
    IRequestHandler<CreateMessageCommand, Message>,
    IRequestHandler<UpdateMessageCommand, bool>,
    IRequestHandler<DeleteMessageCommand, bool>
{
    private readonly IMessageService _service;

    public MessageCommandHandler(IMessageService service)
    {
        _service = service;
    }

    public async Task<Message> Handle(CreateMessageCommand request, CancellationToken ct)
    {
        var entity = new Message
        {
            TypeId = request.TypeId,
            ExpediteurId = request.ExpediteurId,
            DestinataireId = request.DestinataireId,
            Sujet = request.Sujet,
            Contenu = request.Contenu,
            EstLu = request.EstLu,
            DateLecture = request.DateLecture,
            GroupeDestinataire = request.GroupeDestinataire,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateMessageCommand request, CancellationToken ct)
    {
        var entity = new Message
        {
            Id = request.Id,
            TypeId = request.TypeId,
            ExpediteurId = request.ExpediteurId,
            DestinataireId = request.DestinataireId,
            Sujet = request.Sujet,
            Contenu = request.Contenu,
            EstLu = request.EstLu,
            DateLecture = request.DateLecture,
            GroupeDestinataire = request.GroupeDestinataire,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteMessageCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
