using System.Text.Json;
using FluentValidation;
using Inscriptions.Api.Models;

namespace Inscriptions.Api.Middleware;

/// <summary>
/// Middleware global de gestion des exceptions.
/// Mappe les exceptions vers des codes HTTP et retourne des ApiResponse standardisees.
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="GlobalExceptionMiddleware"/>.
    /// </summary>
    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Intercepte les exceptions non gerees et retourne une reponse structuree.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString();

        var (statusCode, message) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message),
            ValidationException validationEx => (StatusCodes.Status400BadRequest, validationEx.Message),
            InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError,
                _environment.IsDevelopment() ? exception.Message : "Une erreur interne est survenue.")
        };

        _logger.LogError(exception,
            "Exception non geree - {ExceptionType}: {Message} [CorrelationId: {CorrelationId}]",
            exception.GetType().Name, exception.Message, correlationId);

        var response = ApiResponse<object>.Fail(message);
        response.CorrelationId = correlationId;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, JsonOptions);
        await context.Response.WriteAsync(json);
    }
}
