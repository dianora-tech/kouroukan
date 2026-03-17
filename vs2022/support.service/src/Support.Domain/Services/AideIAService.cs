using Microsoft.Extensions.Logging;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Support.Domain.Ports.Output;

namespace Support.Domain.Services;

/// <summary>
/// Service d'aide generative IA via Ollama (open-source, self-hosted).
/// Fallback gracieux si Ollama est indisponible.
/// </summary>
public sealed class AideIAService : IAideIAService
{
    private const int MaxMessagesParConversation = 50;
    private const int MaxConversationsActives = 10;
    private const string FallbackMessage = "Le service d'aide IA est temporairement indisponible. " +
        "Veuillez consulter nos articles d'aide pour trouver une reponse a votre question.";

    private readonly IConversationIARepository _conversationRepo;
    private readonly IArticleAideRepository _articleRepo;
    private readonly ITicketRepository _ticketRepo;
    private readonly IOllamaClient _ollamaClient;
    private readonly ILogger<AideIAService> _logger;

    public AideIAService(
        IConversationIARepository conversationRepo,
        IArticleAideRepository articleRepo,
        ITicketRepository ticketRepo,
        IOllamaClient ollamaClient,
        ILogger<AideIAService> logger)
    {
        _conversationRepo = conversationRepo;
        _articleRepo = articleRepo;
        _ticketRepo = ticketRepo;
        _ollamaClient = ollamaClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> GenererReponseAsync(int conversationId, string question, CancellationToken ct = default)
    {
        var conversation = await _conversationRepo.GetByIdAsync(conversationId, ct)
            ?? throw new KeyNotFoundException($"Conversation {conversationId} introuvable.");

        if (!conversation.EstActive)
            throw new InvalidOperationException("Cette conversation est terminee.");

        var messageCount = await _conversationRepo.CountMessagesAsync(conversationId, ct);
        if (messageCount >= MaxMessagesParConversation)
            throw new InvalidOperationException($"Limite de {MaxMessagesParConversation} messages atteinte. Veuillez creer une nouvelle conversation.");

        // Rechercher les articles pertinents pour le contexte RAG
        var articles = await RechercherArticlesContexteAsync(question, 5, ct);
        var contexte = BuildContexteRAG(articles);

        // Sauvegarder le message utilisateur
        var userMessage = new MessageIA
        {
            ConversationIAId = conversationId,
            Role = "user",
            Contenu = question,
            UserId = conversation.UserId
        };
        await _conversationRepo.AddMessageAsync(userMessage, ct);

        // Verifier la disponibilite d'Ollama
        if (!await _ollamaClient.IsAvailableAsync(ct))
        {
            _logger.LogWarning("Ollama est indisponible, utilisation du fallback.");
            var fallbackMsg = new MessageIA
            {
                ConversationIAId = conversationId,
                Role = "assistant",
                Contenu = FallbackMessage,
                ContexteArticlesIds = FormatArticleIds(articles),
                TokensUtilises = 0,
                UserId = conversation.UserId
            };
            await _conversationRepo.AddMessageAsync(fallbackMsg, ct);
            await UpdateNombreMessagesAsync(conversation, ct);
            return FallbackMessage;
        }

        // Recuperer l'historique et generer la reponse
        var historique = await _conversationRepo.GetMessagesAsync(conversationId, ct);
        var messages = historique.Select(m => (m.Role, m.Contenu)).ToList();

        try
        {
            var (response, tokensUsed) = await _ollamaClient.ChatAsync(contexte, messages, ct);

            var assistantMessage = new MessageIA
            {
                ConversationIAId = conversationId,
                Role = "assistant",
                Contenu = response,
                ContexteArticlesIds = FormatArticleIds(articles),
                TokensUtilises = tokensUsed,
                UserId = conversation.UserId
            };
            await _conversationRepo.AddMessageAsync(assistantMessage, ct);
            await UpdateNombreMessagesAsync(conversation, ct);

            _logger.LogInformation("Reponse IA generee pour la conversation {ConversationId} ({Tokens} tokens).",
                conversationId, tokensUsed);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la generation IA pour la conversation {ConversationId}.", conversationId);

            var errorMsg = new MessageIA
            {
                ConversationIAId = conversationId,
                Role = "assistant",
                Contenu = FallbackMessage,
                TokensUtilises = 0,
                UserId = conversation.UserId
            };
            await _conversationRepo.AddMessageAsync(errorMsg, ct);
            await UpdateNombreMessagesAsync(conversation, ct);
            return FallbackMessage;
        }
    }

    /// <inheritdoc />
    public async Task<string> SuggererReponseTicketAsync(int ticketId, CancellationToken ct = default)
    {
        var ticket = await _ticketRepo.GetByIdAsync(ticketId, ct)
            ?? throw new KeyNotFoundException($"Ticket {ticketId} introuvable.");

        if (!await _ollamaClient.IsAvailableAsync(ct))
        {
            _logger.LogWarning("Ollama est indisponible pour la suggestion de reponse au ticket {TicketId}.", ticketId);
            return FallbackMessage;
        }

        var articles = await RechercherArticlesContexteAsync(ticket.Sujet + " " + ticket.Contenu, 5, ct);
        var contexte = BuildContexteRAG(articles);

        var prompt = $"Un utilisateur a soumis le ticket suivant :\n" +
                     $"Sujet : {ticket.Sujet}\n" +
                     $"Categorie : {ticket.CategorieTicket}\n" +
                     $"Priorite : {ticket.Priorite}\n" +
                     $"Description : {ticket.Contenu}\n\n" +
                     $"Propose une reponse professionnelle et utile.";

        var messages = new List<(string Role, string Content)> { ("user", prompt) };

        try
        {
            var (response, _) = await _ollamaClient.ChatAsync(contexte, messages, ct);
            _logger.LogInformation("Suggestion IA generee pour le ticket {TicketId}.", ticketId);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suggestion IA pour le ticket {TicketId}.", ticketId);
            return FallbackMessage;
        }
    }

    /// <inheritdoc />
    public async Task<List<ArticleAide>> RechercherArticlesContexteAsync(string query, int limit = 5, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<ArticleAide>();

        var articles = await _articleRepo.RechercherFullTextAsync(query, limit, ct);
        return articles.ToList();
    }

    /// <inheritdoc />
    public async Task<ConversationIA> CreerConversationAsync(int utilisateurId, int userId, CancellationToken ct = default)
    {
        var activeCount = await _conversationRepo.CountActiveByUserAsync(utilisateurId, ct);
        if (activeCount >= MaxConversationsActives)
            throw new InvalidOperationException($"Limite de {MaxConversationsActives} conversations actives atteinte.");

        var conversation = new ConversationIA
        {
            UtilisateurId = utilisateurId,
            EstActive = true,
            NombreMessages = 0,
            UserId = userId
        };

        var created = await _conversationRepo.AddAsync(conversation, ct);
        _logger.LogInformation("Conversation IA creee (id: {Id}) pour l'utilisateur {UtilisateurId}.",
            created.Id, utilisateurId);
        return created;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MessageIA>> GetMessagesAsync(int conversationId, CancellationToken ct = default)
    {
        var conversation = await _conversationRepo.GetByIdAsync(conversationId, ct)
            ?? throw new KeyNotFoundException($"Conversation {conversationId} introuvable.");

        return await _conversationRepo.GetMessagesAsync(conversationId, ct);
    }

    private static string BuildContexteRAG(List<ArticleAide> articles)
    {
        if (articles.Count == 0)
            return string.Empty;

        var contexte = "Voici des articles de la base de connaissances qui peuvent etre utiles :\n\n";
        foreach (var article in articles)
        {
            contexte += $"--- Article : {article.Titre} ---\n{article.Contenu}\n\n";
        }
        return contexte;
    }

    private static string? FormatArticleIds(List<ArticleAide> articles)
    {
        if (articles.Count == 0)
            return null;
        return System.Text.Json.JsonSerializer.Serialize(articles.Select(a => a.Id));
    }

    private async Task UpdateNombreMessagesAsync(ConversationIA conversation, CancellationToken ct)
    {
        var count = await _conversationRepo.CountMessagesAsync(conversation.Id, ct);
        conversation.NombreMessages = count;
        await _conversationRepo.UpdateAsync(conversation, ct);
    }
}
