using GnDapper.Entities;

namespace GnDapper.Specifications;

/// <summary>
/// Interface definissant une specification de requete pour filtrer, trier et paginer les entites.
/// Traduit en clauses SQL WHERE, ORDER BY et LIMIT/OFFSET pour PostgreSQL.
/// </summary>
/// <typeparam name="T">Type de l'entite cible.</typeparam>
public interface ISpecification<T> where T : IEntity
{
    /// <summary>
    /// Clause WHERE SQL (sans le mot-cle WHERE). Null si aucun filtre.
    /// Exemple : "first_name ILIKE @Name AND is_active = @IsActive".
    /// </summary>
    string? WhereClause { get; }

    /// <summary>
    /// Parametres associes a la clause WHERE.
    /// </summary>
    object? Parameters { get; }

    /// <summary>
    /// Clause ORDER BY SQL (sans le mot-cle ORDER BY). Null si pas de tri.
    /// Exemple : "created_at DESC, last_name ASC".
    /// </summary>
    string? OrderByClause { get; }

    /// <summary>
    /// Nombre d'elements a ignorer (OFFSET). Null si pas de pagination.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Nombre d'elements a retourner (LIMIT). Null si pas de pagination.
    /// </summary>
    int? Take { get; }
}
