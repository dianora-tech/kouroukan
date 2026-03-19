namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Exception lancee quand une ressource est en conflit (ex: doublon email/telephone).
/// Mappee vers HTTP 409 Conflict par GlobalExceptionMiddleware.
/// </summary>
public sealed class ConflictException : Exception
{
    /// <summary>
    /// Initialise une nouvelle instance de <see cref="ConflictException"/>.
    /// </summary>
    public ConflictException(string message) : base(message) { }
}
