using System.Text.RegularExpressions;
using GnDapper.Exceptions;

namespace GnDapper.Guards;

/// <summary>
/// Garde de securite contre les injections SQL dans les requetes brutes.
/// Detecte les patterns dangereux (DROP, TRUNCATE, UNION SELECT, etc.) et leve une exception.
/// </summary>
public static partial class SqlInjectionGuard
{
    /// <summary>
    /// Valide une requete SQL brute et leve une <see cref="DataAccessException"/> si un pattern dangereux est detecte.
    /// </summary>
    /// <param name="sql">Requete SQL a valider.</param>
    /// <exception cref="DataAccessException">Si un pattern SQL dangereux est detecte.</exception>
    /// <exception cref="ArgumentException">Si la requete est null ou vide.</exception>
    public static void Validate(string sql)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql, nameof(sql));

        if (DropPattern().IsMatch(sql))
        {
            throw new DataAccessException(
                "Requete SQL dangereuse detectee : instruction DROP interdite.");
        }

        if (DeleteWithoutWherePattern().IsMatch(sql))
        {
            throw new DataAccessException(
                "Requete SQL dangereuse detectee : DELETE sans clause WHERE interdit.");
        }

        if (TruncatePattern().IsMatch(sql))
        {
            throw new DataAccessException(
                "Requete SQL dangereuse detectee : instruction TRUNCATE interdite.");
        }

        if (SqlCommentPattern().IsMatch(sql))
        {
            throw new DataAccessException(
                "Requete SQL dangereuse detectee : commentaires SQL (--) interdits dans les requetes dynamiques.");
        }

        if (UnionSelectPattern().IsMatch(sql))
        {
            throw new DataAccessException(
                "Requete SQL dangereuse detectee : UNION SELECT interdit dans les requetes dynamiques.");
        }

        if (XpCommandPattern().IsMatch(sql))
        {
            throw new DataAccessException(
                "Requete SQL dangereuse detectee : commande xp_ interdite.");
        }

        if (ExecCommandPattern().IsMatch(sql))
        {
            throw new DataAccessException(
                "Requete SQL dangereuse detectee : instruction EXEC interdite.");
        }
    }

    /// <summary>
    /// Pattern pour detecter les instructions DROP (TABLE, DATABASE, INDEX, SCHEMA).
    /// </summary>
    [GeneratedRegex(@"\bDROP\s+(TABLE|DATABASE|INDEX|SCHEMA)\b", RegexOptions.IgnoreCase)]
    private static partial Regex DropPattern();

    /// <summary>
    /// Pattern pour detecter les DELETE sans clause WHERE.
    /// </summary>
    [GeneratedRegex(@"\bDELETE\s+FROM\s+\w+[\.\w]*\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
    private static partial Regex DeleteWithoutWherePattern();

    /// <summary>
    /// Pattern pour detecter les instructions TRUNCATE.
    /// </summary>
    [GeneratedRegex(@"\bTRUNCATE\s+(TABLE\s+)?\w+", RegexOptions.IgnoreCase)]
    private static partial Regex TruncatePattern();

    /// <summary>
    /// Pattern pour detecter les commentaires SQL (--).
    /// </summary>
    [GeneratedRegex(@"--")]
    private static partial Regex SqlCommentPattern();

    /// <summary>
    /// Pattern pour detecter les UNION SELECT.
    /// </summary>
    [GeneratedRegex(@"\bUNION\s+(ALL\s+)?SELECT\b", RegexOptions.IgnoreCase)]
    private static partial Regex UnionSelectPattern();

    /// <summary>
    /// Pattern pour detecter les commandes xp_ (SQL Server extended procedures).
    /// </summary>
    [GeneratedRegex(@"\bxp_\w+", RegexOptions.IgnoreCase)]
    private static partial Regex XpCommandPattern();

    /// <summary>
    /// Pattern pour detecter les instructions EXEC/EXECUTE.
    /// </summary>
    [GeneratedRegex(@"\bEXEC\s*\(", RegexOptions.IgnoreCase)]
    private static partial Regex ExecCommandPattern();
}
