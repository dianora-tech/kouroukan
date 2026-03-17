using GnDapper.Entities;
using GnDapper.Models;
using GnDapper.Specifications;

namespace GnDapper.Repositories;

/// <summary>
/// Interface generique du repository pour les operations CRUD, les requetes par specification
/// et les operations en masse sur les entites PostgreSQL.
/// </summary>
/// <typeparam name="T">Type de l'entite implementant <see cref="IEntity"/>.</typeparam>
public interface IRepository<T> where T : class, IEntity
{
    // ========================================================================
    // CRUD
    // ========================================================================

    /// <summary>
    /// Recupere une entite par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de l'entite.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>L'entite trouvee ou null si inexistante.</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupere toutes les entites de la table.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Liste en lecture seule de toutes les entites.</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Insere une nouvelle entite et retourne l'entite avec son identifiant genere.
    /// Utilise RETURNING * pour PostgreSQL.
    /// </summary>
    /// <param name="entity">Entite a inserer.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>L'entite inseree avec son identifiant.</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met a jour une entite existante.
    /// </summary>
    /// <param name="entity">Entite avec les valeurs mises a jour.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns><c>true</c> si la mise a jour a reussi, <c>false</c> sinon.</returns>
    Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime une entite par son identifiant.
    /// Pour les entites <see cref="ISoftDeletable"/>, effectue une suppression logique.
    /// </summary>
    /// <param name="id">Identifiant de l'entite a supprimer.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns><c>true</c> si la suppression a reussi, <c>false</c> sinon.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifie si une entite avec l'identifiant specifie existe.
    /// </summary>
    /// <param name="id">Identifiant a verifier.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns><c>true</c> si l'entite existe, <c>false</c> sinon.</returns>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    // ========================================================================
    // Requetes brutes
    // ========================================================================

    /// <summary>
    /// Execute une requete SQL brute et retourne les resultats.
    /// La requete est validee par <see cref="Guards.SqlInjectionGuard"/> si active.
    /// </summary>
    /// <param name="sql">Requete SQL SELECT.</param>
    /// <param name="param">Parametres de la requete.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Liste des entites correspondantes.</returns>
    Task<IReadOnlyList<T>> GetWithQueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute une requete SQL brute et retourne un seul resultat.
    /// La requete est validee par <see cref="Guards.SqlInjectionGuard"/> si active.
    /// </summary>
    /// <param name="sql">Requete SQL SELECT.</param>
    /// <param name="param">Parametres de la requete.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>L'entite trouvee ou null.</returns>
    Task<T?> GetSingleWithQueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default);

    // ========================================================================
    // Specification
    // ========================================================================

    /// <summary>
    /// Recherche les entites correspondant a une specification.
    /// </summary>
    /// <param name="spec">Specification definissant les criteres de recherche.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Liste des entites correspondantes.</returns>
    Task<IReadOnlyList<T>> FindAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recherche les entites correspondant a une specification avec pagination.
    /// </summary>
    /// <param name="spec">Specification definissant les criteres, le tri et la pagination.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Resultat pagine contenant les entites et les metadonnees de pagination.</returns>
    Task<PagedResult<T>> FindPagedAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compte le nombre d'entites correspondant a une specification.
    /// </summary>
    /// <param name="spec">Specification definissant les criteres. Null pour compter toutes les entites.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Nombre d'entites correspondantes.</returns>
    Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default);

    // ========================================================================
    // Operations en masse (Bulk)
    // ========================================================================

    /// <summary>
    /// Insere plusieurs entites dans une seule transaction.
    /// </summary>
    /// <param name="entities">Entites a inserer.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Nombre d'entites inserees.</returns>
    Task<int> BulkInsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met a jour plusieurs entites dans une seule transaction.
    /// </summary>
    /// <param name="entities">Entites a mettre a jour.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Nombre d'entites mises a jour.</returns>
    Task<int> BulkUpdateAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime plusieurs entites par leurs identifiants dans une seule transaction.
    /// </summary>
    /// <param name="ids">Identifiants des entites a supprimer.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Nombre d'entites supprimees.</returns>
    Task<int> BulkDeleteAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
