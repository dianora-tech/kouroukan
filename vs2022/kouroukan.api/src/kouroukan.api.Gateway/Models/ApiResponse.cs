namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Enveloppe standard pour toutes les reponses API.
/// </summary>
/// <typeparam name="T">Type des donnees retournees.</typeparam>
public class ApiResponse<T>
{
    /// <summary>Indique si l'operation a reussi.</summary>
    public bool Success { get; set; }

    /// <summary>Message descriptif.</summary>
    public string? Message { get; set; }

    /// <summary>Donnees retournees.</summary>
    public T? Data { get; set; }

    /// <summary>Liste des erreurs de validation.</summary>
    public IReadOnlyList<string>? Errors { get; set; }

    /// <summary>Identifiant de correlation pour le suivi.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Cree une reponse de succes.</summary>
    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    /// <summary>Cree une reponse d'erreur.</summary>
    public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}
