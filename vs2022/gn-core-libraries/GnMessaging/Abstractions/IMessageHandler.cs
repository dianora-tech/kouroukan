namespace GnMessaging.Abstractions;

/// <summary>
/// Interface pour le traitement d'un type de message specifique.
/// Chaque handler est enregistre via DI et route automatiquement par le consumer.
/// </summary>
/// <typeparam name="T">Type du message a traiter.</typeparam>
public interface IMessageHandler<in T> where T : class, IMessage
{
    /// <summary>
    /// Traite le message recu.
    /// </summary>
    /// <param name="message">Le message a traiter.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task HandleAsync(T message, CancellationToken cancellationToken = default);
}
