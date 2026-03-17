using Microsoft.Extensions.Logging;

namespace GnMessaging.Retry;

/// <summary>
/// Politique de retry avec backoff exponentiel pour le traitement des messages.
/// Delais: 1s, 2s, 4s, 8s, 16s (max 5 tentatives par defaut).
/// </summary>
public sealed class RetryPolicy
{
    private readonly ILogger<RetryPolicy> _logger;

    /// <summary>
    /// Delai de base pour le backoff exponentiel.
    /// </summary>
    public static readonly TimeSpan BaseDelay = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Nombre maximum de tentatives par defaut.
    /// </summary>
    public const int DefaultMaxRetries = 5;

    public RetryPolicy(ILogger<RetryPolicy> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Execute une action avec retry et backoff exponentiel.
    /// </summary>
    /// <param name="action">Action a executer.</param>
    /// <param name="maxRetries">Nombre maximum de tentatives.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <exception cref="AggregateException">Si toutes les tentatives echouent.</exception>
    public async Task ExecuteAsync(Func<Task> action, int maxRetries = DefaultMaxRetries,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var exceptions = new List<Exception>();

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                exceptions.Add(ex);

                var delay = CalculateDelay(attempt);

                _logger.LogWarning(ex,
                    "Tentative {Attempt}/{MaxRetries} echouee. Retry dans {Delay}ms.",
                    attempt + 1, maxRetries, delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        throw new AggregateException(
            $"Echec apres {maxRetries} tentatives.", exceptions);
    }

    /// <summary>
    /// Calcule le delai de backoff exponentiel pour une tentative donnee.
    /// </summary>
    /// <param name="attempt">Numero de la tentative (0-based).</param>
    /// <returns>Delai avant la prochaine tentative.</returns>
    public static TimeSpan CalculateDelay(int attempt)
    {
        // 2^attempt * baseDelay : 1s, 2s, 4s, 8s, 16s
        var delaySeconds = Math.Pow(2, attempt) * BaseDelay.TotalSeconds;
        return TimeSpan.FromSeconds(delaySeconds);
    }
}
