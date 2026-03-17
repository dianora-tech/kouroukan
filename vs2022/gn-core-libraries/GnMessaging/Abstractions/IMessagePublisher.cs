using GnMessaging.Models;

namespace GnMessaging.Abstractions;

/// <summary>
/// Interface pour la publication de messages sur le bus RabbitMQ.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publie un message sur l'exchange specifie avec le routing key donne.
    /// </summary>
    /// <typeparam name="T">Type du message (doit implementer IMessage).</typeparam>
    /// <param name="message">Le message a publier.</param>
    /// <param name="exchange">Nom de l'exchange RabbitMQ.</param>
    /// <param name="routingKey">Cle de routage.</param>
    /// <param name="options">Options de publication optionnelles.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task PublishAsync<T>(T message, string exchange, string routingKey,
        PublishOptions? options = null, CancellationToken cancellationToken = default)
        where T : class, IMessage;
}
