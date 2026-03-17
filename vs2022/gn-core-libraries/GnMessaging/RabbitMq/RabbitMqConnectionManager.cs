using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GnMessaging.RabbitMq;

/// <summary>
/// Gere la connexion persistante a RabbitMQ avec reconnexion automatique.
/// Thread-safe : une seule connexion partagee, un channel par operation.
/// </summary>
public class RabbitMqConnectionManager : IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConnectionManager> _logger;
    private readonly object _lock = new();
    private IConnection? _connection;
    private bool _disposed;

    public RabbitMqConnectionManager(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConnectionManager> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Obtient la connexion RabbitMQ. Cree ou reconnecte si necessaire.
    /// </summary>
    public IConnection GetConnection()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqConnectionManager));

        if (_connection is { IsOpen: true })
            return _connection;

        lock (_lock)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            _connection?.Dispose();
            _connection = CreateConnection();
            return _connection;
        }
    }

    /// <summary>
    /// Cree un nouveau channel RabbitMQ.
    /// </summary>
    public virtual IModel CreateChannel()
    {
        var connection = GetConnection();
        return connection.CreateModel();
    }

    private IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(_options.ReconnectDelaySeconds),
            DispatchConsumersAsync = true
        };

        _logger.LogInformation(
            "Connexion a RabbitMQ {Host}:{Port}/{VHost}...",
            _options.HostName, _options.Port, _options.VirtualHost);

        var connection = factory.CreateConnection($"{_options.ServiceName}-connection");

        connection.ConnectionShutdown += (_, args) =>
        {
            _logger.LogWarning(
                "Connexion RabbitMQ fermee: {Reason}. Reconnexion automatique activee.",
                args.ReplyText);
        };

        if (connection is IAutorecoveringConnection autorecoveringConnection)
        {
            autorecoveringConnection.RecoverySucceeded += (_, _) =>
            {
                _logger.LogInformation("Reconnexion RabbitMQ reussie.");
            };
        }

        _logger.LogInformation("Connexion RabbitMQ etablie.");
        return connection;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _connection?.Dispose();
    }
}
