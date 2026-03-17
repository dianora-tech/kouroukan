namespace GnDapper.Entities;

/// <summary>
/// Interface de base pour toutes les entites du domaine.
/// Chaque entite doit posseder un identifiant unique de type entier.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Identifiant unique de l'entite (cle primaire).
    /// </summary>
    int Id { get; set; }
}
