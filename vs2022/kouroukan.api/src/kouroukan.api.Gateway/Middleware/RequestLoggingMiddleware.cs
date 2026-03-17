using System.Diagnostics;

namespace Kouroukan.Api.Gateway.Middleware;

/// <summary>
/// Middleware de journalisation des requetes HTTP avec mesure de la duree.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="RequestLoggingMiddleware"/>.
    /// </summary>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Intercepte la requete pour logger le debut et la fin avec la duree.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogInformation("HTTP {Method} {Path} - Debut", method, path);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var elapsed = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation(
                "HTTP {Method} {Path} - {StatusCode} en {ElapsedMs}ms",
                method, path, statusCode, elapsed);
        }
    }
}
