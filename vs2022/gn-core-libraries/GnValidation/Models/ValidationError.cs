namespace GnValidation.Models;

/// <summary>
/// Represente une erreur de validation retournee par le pipeline.
/// </summary>
/// <param name="PropertyName">Nom de la propriete en erreur.</param>
/// <param name="ErrorMessage">Message d'erreur en francais.</param>
public record ValidationError(string PropertyName, string ErrorMessage);
