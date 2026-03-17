namespace GnDapper.Options;

/// <summary>
/// Options de configuration pour la couche d'acces aux donnees GnDapper.
/// Liees a la section "GnDapper" du fichier de configuration.
/// </summary>
public class GnDapperOptions
{
    /// <summary>
    /// Chaine de connexion PostgreSQL (Npgsql).
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Delai d'expiration des commandes SQL en secondes. Par defaut : 30.
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Active la protection contre les injections SQL sur les requetes brutes. Par defaut : true.
    /// </summary>
    public bool EnableSqlInjectionGuard { get; set; } = true;
}
