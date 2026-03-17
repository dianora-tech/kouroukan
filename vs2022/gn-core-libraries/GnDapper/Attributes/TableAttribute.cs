namespace GnDapper.Attributes;

/// <summary>
/// Attribut pour specifier le nom de la table PostgreSQL associee a une entite.
/// Supporte les noms qualifies par le schema (ex: "inscriptions.eleves").
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TableAttribute : Attribute
{
    /// <summary>
    /// Nom complet de la table PostgreSQL, incluant le schema si necessaire.
    /// Exemple : "inscriptions.eleves", "auth.users".
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="TableAttribute"/>.
    /// </summary>
    /// <param name="name">Nom de la table PostgreSQL (ex: "schema.table").</param>
    public TableAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        Name = name;
    }
}
