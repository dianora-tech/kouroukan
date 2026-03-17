using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using Documents.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Documents.Domain.Services;

/// <summary>
/// Service metier pour les signatures electroniques.
/// </summary>
public sealed class SignatureService : ISignatureService
{
    private const string Exchange = "kouroukan.events";

    private static readonly string[] StatutsSignatureValides = ["EnAttente", "Signe", "Refuse", "Delegue"];
    private static readonly string[] NiveauxSignatureValides = ["Basique", "Avancee"];

    private readonly ISignatureRepository _repository;
    private readonly IDocumentGenereRepository _documentGenereRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<SignatureService> _logger;

    public SignatureService(
        ISignatureRepository repository,
        IDocumentGenereRepository documentGenereRepository,
        IMessagePublisher publisher,
        ILogger<SignatureService> logger)
    {
        _repository = repository;
        _documentGenereRepository = documentGenereRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Signature?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Signature>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Signature>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Signature> CreateAsync(Signature entity, CancellationToken ct = default)
    {
        if (!StatutsSignatureValides.Contains(entity.StatutSignature))
            throw new InvalidOperationException(
                $"Statut de signature invalide : '{entity.StatutSignature}'.");

        if (!NiveauxSignatureValides.Contains(entity.NiveauSignature))
            throw new InvalidOperationException(
                $"Niveau de signature invalide : '{entity.NiveauSignature}'. Valeurs acceptees : {string.Join(", ", NiveauxSignatureValides)}");

        if (!await _documentGenereRepository.ExistsAsync(entity.DocumentGenereId, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"Le document genere avec l'id {entity.DocumentGenereId} n'existe pas.");

        if (entity.OrdreSignature <= 0)
            throw new InvalidOperationException("L'ordre de signature doit etre superieur a 0.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Signature {Id} creee pour le document {DocumentGenereId} par le signataire {SignataireId}.",
            created.Id, created.DocumentGenereId, created.SignataireId);

        var evt = new EntityCreatedEvent<Signature>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.signature", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Signature entity, CancellationToken ct = default)
    {
        if (!StatutsSignatureValides.Contains(entity.StatutSignature))
            throw new InvalidOperationException(
                $"Statut de signature invalide : '{entity.StatutSignature}'.");

        if (!NiveauxSignatureValides.Contains(entity.NiveauSignature))
            throw new InvalidOperationException(
                $"Niveau de signature invalide : '{entity.NiveauSignature}'.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Signature {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Signature>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.signature", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Signature {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Signature>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.signature", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
