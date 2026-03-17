using Serilog.Context;

namespace Kouroukan.Api.Gateway.Middleware;

/// <summary>
/// Middleware qui genere ou propage un identifiant de correlation (X-Correlation-Id)
/// pour le suivi distribue des requetes.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CorrelationIdMiddleware"/>.
    /// </summary>
    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Intercepte la requete pour generer ou propager le correlation ID.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString("N");

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
