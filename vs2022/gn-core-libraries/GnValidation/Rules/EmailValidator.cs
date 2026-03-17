using System.Net.Mail;
using System.Text.RegularExpressions;

namespace GnValidation.Rules;

/// <summary>
/// Implementation du validateur d'adresses email conforme RFC 5322.
/// Utilise <see cref="MailAddress.TryCreate(string, out MailAddress?)"/> et des verifications supplementaires.
/// </summary>
public sealed partial class EmailValidator : IEmailValidator
{
    /// <inheritdoc />
    public bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Verification via System.Net.Mail (RFC 5322)
        if (!MailAddress.TryCreate(email, out var mailAddress))
            return false;

        // Verifier que l'adresse reconstruite correspond a l'entree
        // (MailAddress accepte des formats comme "Name <email>")
        if (!string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase))
            return false;

        // Verifications supplementaires
        var parts = email.Split('@');
        if (parts.Length != 2)
            return false;

        var localPart = parts[0];
        var domainPart = parts[1];

        // Pas de points consecutifs dans la partie locale
        if (localPart.Contains(".."))
            return false;

        // Le domaine doit contenir au moins un point
        if (!domainPart.Contains('.'))
            return false;

        // Le TLD doit avoir au moins 2 caracteres
        var tld = domainPart.Split('.').Last();
        if (tld.Length < 2)
            return false;

        // Verification regex supplementaire pour les caracteres valides
        if (!StrictEmailRegex().IsMatch(email))
            return false;

        return true;
    }

    [GeneratedRegex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", RegexOptions.Compiled)]
    private static partial Regex StrictEmailRegex();
}
