using System.Reflection;
using FluentValidation;
using GnValidation.Behaviors;
using GnValidation.Options;
using GnValidation.Rules;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GnValidation.Extensions;

/// <summary>
/// Extensions pour l'enregistrement des services GnValidation dans le conteneur DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre les services de validation GnValidation (regles, sanitizer, validateurs FluentValidation).
    /// <para>
    /// Services enregistres :
    /// <list type="bullet">
    ///   <item><description><see cref="IPhoneNumberValidator"/> → <see cref="PhoneNumberValidator"/> (Singleton)</description></item>
    ///   <item><description><see cref="IEmailValidator"/> → <see cref="EmailValidator"/> (Singleton)</description></item>
    ///   <item><description><see cref="ICoordinatesValidator"/> → <see cref="CoordinatesValidator"/> (Singleton)</description></item>
    ///   <item><description><see cref="IPasswordStrengthValidator"/> → <see cref="PasswordStrengthValidator"/> (Singleton)</description></item>
    ///   <item><description><see cref="IStringSanitizer"/> → <see cref="StringSanitizer"/> (Singleton)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Decouvre automatiquement les validateurs FluentValidation dans l'assembly de GnValidation
    /// et dans l'assembly optionnelle specifiee.
    /// </para>
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <param name="configuration">Configuration de l'application (section "GnValidation" optionnelle).</param>
    /// <param name="validatorsAssembly">Assembly supplementaire contenant des validateurs a decouvrir. Null = aucune assembly supplementaire.</param>
    /// <returns>La collection de services pour le chainage.</returns>
    public static IServiceCollection AddGnValidation(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly? validatorsAssembly = null)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        // Configuration
        services.Configure<GnValidationOptions>(configuration.GetSection("GnValidation"));

        // Regles — Singleton (stateless, thread-safe)
        services.AddSingleton<IPhoneNumberValidator, PhoneNumberValidator>();
        services.AddSingleton<IEmailValidator, EmailValidator>();
        services.AddSingleton<ICoordinatesValidator, CoordinatesValidator>();
        services.AddSingleton<IPasswordStrengthValidator, PasswordStrengthValidator>();
        services.AddSingleton<IStringSanitizer, StringSanitizer>();

        // Auto-decouvrir les validateurs FluentValidation de GnValidation
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        // Auto-decouvrir les validateurs de l'assembly du consommateur (si specifiee)
        if (validatorsAssembly is not null)
        {
            services.AddValidatorsFromAssembly(validatorsAssembly);
        }

        return services;
    }

    /// <summary>
    /// Enregistre le pipeline de validation MediatR.
    /// A appeler en complement de <see cref="AddGnValidation"/> si le microservice utilise MediatR.
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <returns>La collection de services pour le chainage.</returns>
    public static IServiceCollection AddGnValidationPipeline(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}
