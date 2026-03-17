using Support.Domain.Entities;

namespace Support.Domain.Ports.Input;

/// <summary>
/// Service d'aide generative IA via Ollama.
/// </summary>
public interface IAideIAService
{
    /// <summary>
    /// Genere une reponse IA dans une conversation existante.
    /// </summary>
    Task<string> GenererReponseAsync(int conversationId, string question, CancellationToken ct = default);

    /// <summary>
    /// Suggere une reponse IA pour un ticket de support.
    /// </summary>
    Task<string> SuggererReponseTicketAsync(int ticketId, CancellationToken ct = default);

    /// <summary>
    /// Recherche les articles d'aide pertinents pour un contexte RAG.
    /// </summary>
    Task<List<ArticleAide>> RechercherArticlesContexteAsync(string query, int limit = 5, CancellationToken ct = default);

    /// <summary>
    /// Cree une nouvelle conversation IA pour un utilisateur.
    /// </summary>
    Task<ConversationIA> CreerConversationAsync(int utilisateurId, int userId, CancellationToken ct = default);

    /// <summary>
    /// Recupere l'historique des messages d'une conversation.
    /// </summary>
    Task<IReadOnlyList<MessageIA>> GetMessagesAsync(int conversationId, CancellationToken ct = default);
}
