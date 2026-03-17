using GnDapper.Connection;
using GnDapper.Options;
using GnDapper.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Support.Domain.Entities;
using Support.Domain.Ports.Output;
using Support.Infrastructure.Dtos;
using Support.Infrastructure.Mappers;

namespace Support.Infrastructure.Repositories;

/// <summary>
/// Repository pour les conversations et messages IA (PostgreSQL).
/// </summary>
public sealed class ConversationIARepository : IConversationIARepository
{
    private readonly AuditRepository<ConversationIADto> _repo;
    private readonly AuditRepository<MessageIADto> _messageRepo;

    public ConversationIARepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<ConversationIADto>> logger,
        ILogger<Repository<MessageIADto>> messageLogger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<ConversationIADto>(connectionFactory, logger, options, httpContextAccessor);
        _messageRepo = new AuditRepository<MessageIADto>(connectionFactory, messageLogger, options, httpContextAccessor);
    }

    public async Task<ConversationIA?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct);
        return dto is null ? null : ConversationIAMapper.ToEntity(dto);
    }

    public async Task<ConversationIA> AddAsync(ConversationIA entity, CancellationToken ct = default)
    {
        var dto = ConversationIAMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct);
        return ConversationIAMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(ConversationIA entity, CancellationToken ct = default)
    {
        var dto = ConversationIAMapper.ToDto(entity);
        return await _repo.UpdateAsync(dto, ct);
    }

    public async Task<int> CountActiveByUserAsync(int utilisateurId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT * FROM support.conversations_ias
            WHERE utilisateur_id = @UtilisateurId AND est_active = TRUE AND is_deleted = FALSE";
        var dtos = await _repo.GetWithQueryAsync(sql, new { UtilisateurId = utilisateurId }, ct);
        return dtos.Count();
    }

    public async Task<IReadOnlyList<MessageIA>> GetMessagesAsync(int conversationId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT * FROM support.message_ias
            WHERE conversation_ia_id = @ConversationId AND is_deleted = FALSE
            ORDER BY created_at ASC";
        var dtos = await _messageRepo.GetWithQueryAsync(sql, new { ConversationId = conversationId }, ct);
        return dtos.Select(MessageIAMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<MessageIA> AddMessageAsync(MessageIA message, CancellationToken ct = default)
    {
        var dto = MessageIAMapper.ToDto(message);
        var created = await _messageRepo.AddAsync(dto, ct);
        return MessageIAMapper.ToEntity(created);
    }

    public async Task<int> CountMessagesAsync(int conversationId, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT * FROM support.message_ias
            WHERE conversation_ia_id = @ConversationId AND is_deleted = FALSE";
        var dtos = await _messageRepo.GetWithQueryAsync(sql, new { ConversationId = conversationId }, ct);
        return dtos.Count();
    }

    public async Task<int> GetTotalConversationsActivesAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM support.conversations_ias WHERE est_active = TRUE AND is_deleted = FALSE";
        var dtos = await _repo.GetWithQueryAsync(sql, null, ct);
        return dtos.Count();
    }

    public async Task<long> GetTotalTokensConsommesAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT * FROM support.message_ias
            WHERE tokens_utilises IS NOT NULL AND is_deleted = FALSE";
        var dtos = await _messageRepo.GetWithQueryAsync(sql, null, ct);
        return dtos.Sum(d => d.TokensUtilises ?? 0);
    }
}
