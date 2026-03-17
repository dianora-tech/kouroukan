using GnValidation.Commands.Support;

namespace GnValidation.FluentValidation.Support;

/// <summary>
/// Validateur pour la creation d'une conversation IA.
/// Verifie l'utilisateur et le titre optionnel.
/// </summary>
public sealed class CreateConversationIAValidator : BaseEntityValidator<CreateConversationIACommand>
{
    public CreateConversationIAValidator()
    {
        RuleForRequiredFk(x => x.UtilisateurId, "utilisateur");

        RuleForOptionalString(x => x.Titre, 200, "titre");
    }
}
