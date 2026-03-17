using GnMessaging.Abstractions;
using GnMessaging.RabbitMq;
using GnMessaging.Retry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GnMessaging.Extensions;

/// <summary>
/// Extensions pour l'enregistrement des services GnMessaging dans le conteneur DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Ajoute les services GnMessaging (RabbitMQ publisher, consumer, retry, DLQ).
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <param name="configuration">Configuration de l'application.</param>
    /// <returns>La collection de services pour chaining.</returns>
    public static IServiceCollection AddGnMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));

        // Connexion
        services.AddSingleton<RabbitMqConnectionManager>();

        // Publisher
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

        // Consumer (BackgroundService)
        services.AddSingleton<RabbitMqConsumer>();
        services.AddSingleton<IMessageConsumer>(sp => sp.GetRequiredService<RabbitMqConsumer>());
        services.AddHostedService(sp => sp.GetRequiredService<RabbitMqConsumer>());

        // Retry & DLQ
        services.AddSingleton<RetryPolicy>();
        services.AddSingleton<DeadLetterHandler>();

        return services;
    }

    /// <summary>
    /// Enregistre un handler pour un type de message specifique.
    /// Le handler est cree dans un scope DI a chaque message recu.
    /// </summary>
    /// <typeparam name="TMessage">Type du message.</typeparam>
    /// <typeparam name="THandler">Type du handler.</typeparam>
    /// <param name="services">Collection de services.</param>
    /// <returns>La collection de services pour chaining.</returns>
    public static IServiceCollection AddMessageHandler<TMessage, THandler>(
        this IServiceCollection services)
        where TMessage : class, IMessage
        where THandler : class, IMessageHandler<TMessage>
    {
        services.AddScoped<IMessageHandler<TMessage>, THandler>();
        return services;
    }
}
