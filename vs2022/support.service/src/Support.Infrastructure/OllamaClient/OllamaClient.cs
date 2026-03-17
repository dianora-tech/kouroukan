using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Support.Domain.Ports.Output;

namespace Support.Infrastructure.OllamaClient;

/// <summary>
/// Client HTTP pour communiquer avec l'API Ollama (open-source, self-hosted).
/// </summary>
public sealed class OllamaClient : IOllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;
    private readonly ILogger<OllamaClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public OllamaClient(
        HttpClient httpClient,
        IOptions<OllamaOptions> options,
        ILogger<OllamaClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(120);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<(string Response, int TokensUsed)> ChatAsync(
        string systemPrompt,
        IReadOnlyList<(string Role, string Content)> messages,
        CancellationToken ct = default)
    {
        var fullSystemPrompt = _options.SystemPrompt;
        if (!string.IsNullOrWhiteSpace(systemPrompt))
        {
            fullSystemPrompt += "\n\n" + systemPrompt;
        }

        var ollamaMessages = new List<OllamaMessage>
        {
            new() { Role = "system", Content = fullSystemPrompt }
        };

        foreach (var (role, content) in messages)
        {
            ollamaMessages.Add(new OllamaMessage { Role = role, Content = content });
        }

        var request = new OllamaChatRequest
        {
            Model = _options.Model,
            Messages = ollamaMessages,
            Stream = false,
            Options = new OllamaRequestOptions
            {
                NumPredict = _options.MaxTokens,
                Temperature = _options.Temperature
            }
        };

        _logger.LogDebug("Envoi de la requete a Ollama ({Model}, {MessageCount} messages).",
            _options.Model, ollamaMessages.Count);

        var response = await _httpClient.PostAsJsonAsync("/api/chat", request, JsonOptions, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(JsonOptions, ct);
        if (result is null)
            throw new InvalidOperationException("Reponse Ollama invalide.");

        var tokensUsed = (result.PromptEvalCount ?? 0) + (result.EvalCount ?? 0);

        _logger.LogInformation("Reponse Ollama recue ({Tokens} tokens).", tokensUsed);

        return (result.Message?.Content ?? string.Empty, tokensUsed);
    }

    /// <inheritdoc />
    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags", ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ollama n'est pas disponible.");
            return false;
        }
    }

    #region Ollama API Models

    private sealed class OllamaChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<OllamaMessage> Messages { get; set; } = [];

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        [JsonPropertyName("options")]
        public OllamaRequestOptions? Options { get; set; }
    }

    private sealed class OllamaMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private sealed class OllamaRequestOptions
    {
        [JsonPropertyName("num_predict")]
        public int NumPredict { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }
    }

    private sealed class OllamaChatResponse
    {
        [JsonPropertyName("message")]
        public OllamaMessage? Message { get; set; }

        [JsonPropertyName("prompt_eval_count")]
        public int? PromptEvalCount { get; set; }

        [JsonPropertyName("eval_count")]
        public int? EvalCount { get; set; }
    }

    #endregion
}
