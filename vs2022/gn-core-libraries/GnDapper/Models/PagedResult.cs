namespace GnDapper.Models;

/// <summary>
/// Resultat pagine contenant une liste d'elements et les metadonnees de pagination.
/// </summary>
/// <typeparam name="T">Type des elements dans la page.</typeparam>
/// <param name="Items">Liste des elements de la page courante.</param>
/// <param name="TotalCount">Nombre total d'elements correspondant aux criteres.</param>
/// <param name="Page">Numero de la page courante (commence a 1).</param>
/// <param name="PageSize">Nombre d'elements par page.</param>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    /// <summary>
    /// Nombre total de pages disponibles.
    /// </summary>
    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling((double)TotalCount / PageSize)
        : 0;

    /// <summary>
    /// Indique s'il existe une page precedente.
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// Indique s'il existe une page suivante.
    /// </summary>
    public bool HasNext => Page < TotalPages;
}
