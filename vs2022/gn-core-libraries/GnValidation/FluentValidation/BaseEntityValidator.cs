using System.Linq.Expressions;
using FluentValidation;

namespace GnValidation.FluentValidation;

/// <summary>
/// Validateur de base fournissant des methodes d'aide pour les regles de validation communes.
/// Tous les messages d'erreur sont en francais.
/// </summary>
/// <typeparam name="T">Type de l'entite a valider.</typeparam>
public abstract class BaseEntityValidator<T> : AbstractValidator<T>
{
    /// <summary>
    /// Ajoute une regle NotEmpty + MaximumLength pour un champ string non nullable.
    /// </summary>
    protected IRuleBuilderOptions<T, string> RuleForRequiredString(
        Expression<Func<T, string>> expression, int maxLength, string fieldName)
    {
        return RuleFor(expression)
            .NotEmpty().WithMessage($"Le champ {fieldName} est obligatoire")
            .MaximumLength(maxLength).WithMessage($"Le champ {fieldName} ne peut pas depasser {maxLength} caracteres");
    }

    /// <summary>
    /// Ajoute une regle MaximumLength pour un champ string nullable.
    /// </summary>
    protected IRuleBuilderOptions<T, string?> RuleForOptionalString(
        Expression<Func<T, string?>> expression, int maxLength, string fieldName)
    {
        return RuleFor(expression)
            .MaximumLength(maxLength).WithMessage($"Le champ {fieldName} ne peut pas depasser {maxLength} caracteres");
    }

    /// <summary>
    /// Ajoute une regle GreaterThan(0) pour un champ FK obligatoire (int).
    /// </summary>
    protected IRuleBuilderOptions<T, int> RuleForRequiredFk(
        Expression<Func<T, int>> expression, string fieldName)
    {
        return RuleFor(expression)
            .GreaterThan(0).WithMessage($"Le champ {fieldName} est obligatoire");
    }

    /// <summary>
    /// Ajoute une regle GreaterThanOrEqualTo(0) pour un montant monetaire.
    /// </summary>
    protected IRuleBuilderOptions<T, decimal> RuleForMoney(
        Expression<Func<T, decimal>> expression, string fieldName)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(0).WithMessage($"Le champ {fieldName} ne peut pas etre negatif");
    }

    /// <summary>
    /// Ajoute une regle de validation d'URL pour un champ string nullable.
    /// </summary>
    protected void RuleForUrl(
        Expression<Func<T, string?>> expression, string fieldName)
    {
        RuleFor(expression)
            .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage($"Le champ {fieldName} doit etre une URL valide");
    }

    /// <summary>
    /// Ajoute une regle Must pour un champ enum-like string non nullable.
    /// </summary>
    protected IRuleBuilderOptions<T, string> RuleForEnum(
        Expression<Func<T, string>> expression, string[] allowedValues, string fieldName)
    {
        return RuleFor(expression)
            .NotEmpty().WithMessage($"Le champ {fieldName} est obligatoire")
            .Must(x => allowedValues.Contains(x))
            .WithMessage($"Le champ {fieldName} doit etre l'une des valeurs suivantes : {string.Join(", ", allowedValues)}");
    }

    /// <summary>
    /// Ajoute une regle Must pour un champ enum-like string nullable.
    /// </summary>
    protected void RuleForOptionalEnum(
        Expression<Func<T, string?>> expression, string[] allowedValues, string fieldName)
    {
        RuleFor(expression)
            .Must(x => string.IsNullOrEmpty(x) || allowedValues.Contains(x))
            .WithMessage($"Le champ {fieldName} doit etre l'une des valeurs suivantes : {string.Join(", ", allowedValues)}");
    }
}
