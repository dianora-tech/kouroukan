using GnMessaging.Models;

namespace GnMessaging.Abstractions;

/// <summary>
/// Interface pour la consommation de messages depuis le bus RabbitMQ.
/// </summary>
public interface IMessageConsumer
{
    /// <summary>
    /// Demarre la consommation de messages depuis la queue specifiee.
    /// </summary>
    /// <typeparam name="T">Type du message attendu.</typeparam>
    /// <param name="options">Options de consommation (nom de queue, prefetch, etc.).</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task StartConsumingAsync<T>(ConsumeOptions options, CancellationToken cancellationToken = default)
        where T : class, IMessage;
}
