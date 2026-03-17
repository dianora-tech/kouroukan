namespace Support.Domain.Ports.Output;

/// <summary>
/// Client HTTP pour communiquer avec l'API Ollama.
/// </summary>
public interface IOllamaClient
{
    /// <summary>
    /// Envoie un prompt a Ollama et retourne la reponse generee.
    /// </summary>
    /// <param name="systemPrompt">Prompt systeme avec contexte RAG.</param>
    /// <param name="messages">Historique des messages (role + contenu).</param>
    /// <param name="ct">Token d'annulation.</param>
    /// <returns>Reponse generee et nombre de tokens utilises.</returns>
    Task<(string Response, int TokensUsed)> ChatAsync(
        string systemPrompt,
        IReadOnlyList<(string Role, string Content)> messages,
        CancellationToken ct = default);

    /// <summary>
    /// Verifie si Ollama est disponible.
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken ct = default);
}
