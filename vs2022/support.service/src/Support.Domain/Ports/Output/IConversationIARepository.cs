using Support.Domain.Entities;

namespace Support.Domain.Ports.Output;

/// <summary>
/// Repository pour les conversations et messages IA.
/// </summary>
public interface IConversationIARepository
{
    Task<ConversationIA?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ConversationIA> AddAsync(ConversationIA entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(ConversationIA entity, CancellationToken ct = default);
    Task<int> CountActiveByUserAsync(int utilisateurId, CancellationToken ct = default);

    Task<IReadOnlyList<MessageIA>> GetMessagesAsync(int conversationId, CancellationToken ct = default);
    Task<MessageIA> AddMessageAsync(MessageIA message, CancellationToken ct = default);
    Task<int> CountMessagesAsync(int conversationId, CancellationToken ct = default);
    Task<int> GetTotalConversationsActivesAsync(CancellationToken ct = default);
    Task<long> GetTotalTokensConsommesAsync(CancellationToken ct = default);
}
