using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using Finances.Domain.Ports.Output;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;

namespace Finances.Domain.Services;

/// <summary>
/// Logique metier pour le journal financier.
/// </summary>
public sealed class JournalFinancierService : IJournalFinancierService
{
    private const string Exchange = "kouroukan.events";
    private readonly IJournalFinancierRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<JournalFinancierService> _logger;

    public JournalFinancierService(
        IJournalFinancierRepository repository,
        IMessagePublisher publisher,
        ILogger<JournalFinancierService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<JournalFinancier?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<JournalFinancier>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<JournalFinancier>> GetPagedAsync(
        int page, int pageSize, int? companyId, string? type, string? categorie,
        DateTime? dateDebut, DateTime? dateFin, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, companyId, type, categorie, dateDebut, dateFin, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<JournalFinancier> CreateAsync(JournalFinancier entity, CancellationToken ct = default)
    {
        if (entity.Montant <= 0)
            throw new InvalidOperationException("Le montant doit etre superieur a 0.");

        if (string.IsNullOrWhiteSpace(entity.Type))
            throw new InvalidOperationException("Le type d'operation est obligatoire.");

        if (entity.DateOperation == default)
            entity.DateOperation = DateTime.UtcNow;

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Entree journal financier creee avec l'id {Id} - {Type} {Montant} pour l'etablissement {CompanyId}.",
            created.Id, created.Type, created.Montant, created.CompanyId);

        var evt = new EntityCreatedEvent<JournalFinancier>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.journal-financier", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }
}
