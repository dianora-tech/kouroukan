namespace GnMessaging.RabbitMq;

/// <summary>
/// Configuration de la connexion RabbitMQ.
/// Binds sur la section "RabbitMq" de appsettings.json.
/// </summary>
public sealed class RabbitMqOptions
{
    /// <summary>
    /// Cle de la section de configuration.
    /// </summary>
    public const string SectionName = "RabbitMq";

    /// <summary>
    /// Nom d'hote du serveur RabbitMQ. Defaut: localhost.
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// Port du serveur RabbitMQ. Defaut: 5672.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Nom d'utilisateur. Defaut: guest.
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// Mot de passe. Defaut: guest.
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Virtual host. Defaut: /.
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// Slug du projet pour le nommage des exchanges (ex: "kouroukan").
    /// </summary>
    public string ProjectSlug { get; set; } = string.Empty;

    /// <summary>
    /// Nom du service courant (utilise comme source dans les enveloppes).
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Delai de reconnexion en secondes. Defaut: 5.
    /// </summary>
    public int ReconnectDelaySeconds { get; set; } = 5;

    /// <summary>
    /// Nom de l'exchange par defaut. Si vide, construit comme "{ProjectSlug}.events".
    /// </summary>
    public string DefaultExchange => string.IsNullOrWhiteSpace(_defaultExchange)
        ? $"{ProjectSlug}.events"
        : _defaultExchange;

    private string _defaultExchange = string.Empty;

    /// <summary>
    /// Setter pour override l'exchange par defaut.
    /// </summary>
    public string DefaultExchangeOverride { set => _defaultExchange = value; }
}
