namespace Support.Infrastructure.OllamaClient;

/// <summary>
/// Configuration pour le client Ollama.
/// </summary>
public sealed class OllamaOptions
{
    public string BaseUrl { get; set; } = "http://ollama:11434";
    public string Model { get; set; } = "mistral";
    public int MaxTokens { get; set; } = 2048;
    public double Temperature { get; set; } = 0.3;
    public string SystemPrompt { get; set; } = "Tu es un assistant d'aide pour la plateforme Kouroukan, une plateforme de gestion scolaire. Reponds en francais de maniere claire et concise. Utilise le contexte des articles d'aide fournis pour repondre aux questions.";
}
