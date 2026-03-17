using System.Data.Common;

namespace GnDapper.Connection;

/// <summary>
/// Fabrique de connexions a la base de donnees PostgreSQL.
/// Fournit des connexions Npgsql pour les operations Dapper.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Cree une nouvelle connexion a la base de donnees.
    /// La connexion n'est PAS ouverte — Dapper gere l'ouverture automatiquement.
    /// </summary>
    /// <returns>Une nouvelle instance de <see cref="DbConnection"/>.</returns>
    DbConnection CreateConnection();
}
