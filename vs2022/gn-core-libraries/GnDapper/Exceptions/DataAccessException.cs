namespace GnDapper.Exceptions;

/// <summary>
/// Exception levee lorsqu'une erreur survient dans la couche d'acces aux donnees.
/// Utilisee pour encapsuler les erreurs SQL, les violations de securite et les problemes de connexion.
/// </summary>
public sealed class DataAccessException : Exception
{
    /// <summary>
    /// Initialise une nouvelle instance de <see cref="DataAccessException"/>.
    /// </summary>
    public DataAccessException()
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="DataAccessException"/> avec un message.
    /// </summary>
    /// <param name="message">Message decrivant l'erreur.</param>
    public DataAccessException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="DataAccessException"/> avec un message et une exception interne.
    /// </summary>
    /// <param name="message">Message decrivant l'erreur.</param>
    /// <param name="innerException">Exception interne a l'origine de l'erreur.</param>
    public DataAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
