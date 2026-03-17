using System.Dynamic;
using GnDapper.Entities;

namespace GnDapper.Specifications;

/// <summary>
/// Implementation de base du pattern Specification avec une API fluide.
/// Permet de construire des requetes SQL typees avec filtres, tri et pagination.
/// </summary>
/// <typeparam name="T">Type de l'entite cible.</typeparam>
/// <example>
/// <code>
/// var spec = new BaseSpecification&lt;Eleve&gt;()
///     .Where("first_name ILIKE @Name", new { Name = "%ibrahima%" })
///     .OrderByAsc("last_name")
///     .WithPaging(1, 20);
/// </code>
/// </example>
public class BaseSpecification<T> : ISpecification<T> where T : IEntity
{
    private readonly List<string> _whereClauses = [];
    private readonly ExpandoObject _parameters = new();
    private readonly List<string> _orderByClauses = [];

    /// <inheritdoc />
    public string? WhereClause => _whereClauses.Count > 0
        ? string.Join(" AND ", _whereClauses)
        : null;

    /// <inheritdoc />
    public object? Parameters => ((IDictionary<string, object?>)_parameters).Count > 0
        ? _parameters
        : null;

    /// <inheritdoc />
    public string? OrderByClause => _orderByClauses.Count > 0
        ? string.Join(", ", _orderByClauses)
        : null;

    /// <inheritdoc />
    public int? Skip { get; private set; }

    /// <inheritdoc />
    public int? Take { get; private set; }

    /// <summary>
    /// Ajoute une condition WHERE a la specification.
    /// Plusieurs appels sont combines avec AND.
    /// </summary>
    /// <param name="clause">Clause SQL (ex: "first_name ILIKE @Name").</param>
    /// <param name="parameters">Parametres associes a la clause (objet anonyme).</param>
    /// <returns>L'instance courante pour le chainage fluide.</returns>
    public BaseSpecification<T> Where(string clause, object? parameters = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clause, nameof(clause));

        _whereClauses.Add(clause);

        if (parameters is not null)
        {
            var dict = (IDictionary<string, object?>)_parameters;
            foreach (var prop in parameters.GetType().GetProperties())
            {
                dict[prop.Name] = prop.GetValue(parameters);
            }
        }

        return this;
    }

    /// <summary>
    /// Ajoute un tri ascendant sur la colonne specifiee.
    /// </summary>
    /// <param name="column">Nom de la colonne PostgreSQL (snake_case).</param>
    /// <returns>L'instance courante pour le chainage fluide.</returns>
    public BaseSpecification<T> OrderByAsc(string column)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(column, nameof(column));
        _orderByClauses.Add($"{column} ASC");
        return this;
    }

    /// <summary>
    /// Ajoute un tri descendant sur la colonne specifiee.
    /// </summary>
    /// <param name="column">Nom de la colonne PostgreSQL (snake_case).</param>
    /// <returns>L'instance courante pour le chainage fluide.</returns>
    public BaseSpecification<T> OrderByDesc(string column)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(column, nameof(column));
        _orderByClauses.Add($"{column} DESC");
        return this;
    }

    /// <summary>
    /// Definit la pagination avec un numero de page et une taille de page.
    /// </summary>
    /// <param name="page">Numero de la page (commence a 1).</param>
    /// <param name="pageSize">Nombre d'elements par page.</param>
    /// <returns>L'instance courante pour le chainage fluide.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Si page ou pageSize sont inferieurs a 1.</exception>
    public BaseSpecification<T> WithPaging(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1, nameof(page));
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1, nameof(pageSize));

        Skip = (page - 1) * pageSize;
        Take = pageSize;
        return this;
    }
}
