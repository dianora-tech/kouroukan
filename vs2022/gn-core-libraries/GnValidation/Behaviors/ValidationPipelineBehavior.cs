using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GnValidation.Behaviors;

/// <summary>
/// Comportement de pipeline MediatR qui execute la validation FluentValidation
/// avant chaque handler de requete. Leve une <see cref="ValidationException"/>
/// si des erreurs sont detectees.
/// </summary>
/// <typeparam name="TRequest">Type de la requete MediatR.</typeparam>
/// <typeparam name="TResponse">Type de la reponse MediatR.</typeparam>
public sealed class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationPipelineBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initialise le comportement de validation avec les validateurs et le logger.
    /// </summary>
    /// <param name="validators">Collection de validateurs FluentValidation pour le type de requete.</param>
    /// <param name="logger">Logger pour tracer les erreurs de validation.</param>
    public ValidationPipelineBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next, nameof(next));

        if (!_validators.Any())
            return await next().ConfigureAwait(false);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)))
            .ConfigureAwait(false);

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count > 0)
        {
            _logger.LogWarning(
                "Validation echouee pour {RequestType} avec {ErrorCount} erreur(s)",
                typeof(TRequest).Name,
                failures.Count);

            throw new ValidationException(failures);
        }

        return await next().ConfigureAwait(false);
    }
}
