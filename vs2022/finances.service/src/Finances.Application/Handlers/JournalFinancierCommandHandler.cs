using Finances.Application.Commands;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using MediatR;

namespace Finances.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour le journal financier.
/// </summary>
public sealed class JournalFinancierCommandHandler :
    IRequestHandler<CreateJournalFinancierCommand, JournalFinancier>
{
    private readonly IJournalFinancierService _service;

    public JournalFinancierCommandHandler(IJournalFinancierService service)
    {
        _service = service;
    }

    public async Task<JournalFinancier> Handle(CreateJournalFinancierCommand request, CancellationToken ct)
    {
        var entity = new JournalFinancier
        {
            CompanyId = request.CompanyId,
            Type = request.Type,
            Categorie = request.Categorie,
            Montant = request.Montant,
            Description = request.Description,
            ReferenceExterne = request.ReferenceExterne,
            ModePaiement = request.ModePaiement,
            DateOperation = request.DateOperation ?? DateTime.UtcNow,
            EleveId = request.EleveId,
            ParentUserId = request.ParentUserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }
}
