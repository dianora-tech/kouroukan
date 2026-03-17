namespace GnValidation.Models;

/// <summary>
/// Resultat de l'analyse de la force d'un mot de passe.
/// </summary>
/// <param name="IsValid">Indique si le mot de passe respecte tous les criteres.</param>
/// <param name="Score">Score de force (0 a 5).</param>
/// <param name="Suggestions">Liste de suggestions d'amelioration en francais.</param>
public record PasswordStrengthResult(bool IsValid, int Score, IReadOnlyList<string> Suggestions);
