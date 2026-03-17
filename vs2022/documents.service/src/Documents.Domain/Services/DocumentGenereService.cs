using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using Documents.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Documents.Domain.Services;

/// <summary>
/// Service metier pour les documents generes.
/// </summary>
public sealed class DocumentGenereService : IDocumentGenereService
{
    private const string Exchange = "kouroukan.events";

    private static readonly string[] StatutsSignatureValides = ["EnAttente", "EnCours", "Signe", "Refuse"];

    private readonly IDocumentGenereRepository _repository;
    private readonly IModeleDocumentRepository _modeleDocumentRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<DocumentGenereService> _logger;

    public DocumentGenereService(
        IDocumentGenereRepository repository,
        IModeleDocumentRepository modeleDocumentRepository,
        IMessagePublisher publisher,
        ILogger<DocumentGenereService> logger)
    {
        _repository = repository;
        _modeleDocumentRepository = modeleDocumentRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<DocumentGenere?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DocumentGenere>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<DocumentGenere>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<DocumentGenere> CreateAsync(DocumentGenere entity, CancellationToken ct = default)
    {
        if (!StatutsSignatureValides.Contains(entity.StatutSignature))
            throw new InvalidOperationException(
                $"Statut de signature invalide : '{entity.StatutSignature}'.");

        if (!await _modeleDocumentRepository.ExistsAsync(entity.ModeleDocumentId, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"Le modele de document avec l'id {entity.ModeleDocumentId} n'existe pas.");

        if (string.IsNullOrWhiteSpace(entity.DonneesJson))
            throw new InvalidOperationException("Les donnees JSON de merge sont obligatoires.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Document genere {Id} cree a partir du modele {ModeleDocumentId}.",
            created.Id, created.ModeleDocumentId);

        var evt = new EntityCreatedEvent<DocumentGenere>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.documentgenere", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(DocumentGenere entity, CancellationToken ct = default)
    {
        if (!StatutsSignatureValides.Contains(entity.StatutSignature))
            throw new InvalidOperationException(
                $"Statut de signature invalide : '{entity.StatutSignature}'.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Document genere {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<DocumentGenere>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.documentgenere", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Document genere {Id} supprime.", id);

            var evt = new EntityDeletedEvent<DocumentGenere>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.documentgenere", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
