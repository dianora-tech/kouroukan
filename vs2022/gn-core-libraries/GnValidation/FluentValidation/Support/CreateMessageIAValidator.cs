using GnValidation.Commands.Support;
using FluentValidation;

namespace GnValidation.FluentValidation.Support;

/// <summary>
/// Validateur pour la creation d'un message IA.
/// Verifie la conversation IA, le role, le contenu
/// et le nombre de tokens utilises.
/// </summary>
public sealed class CreateMessageIAValidator : BaseEntityValidator<CreateMessageIACommand>
{
    public CreateMessageIAValidator()
    {
        RuleForRequiredFk(x => x.ConversationIAId, "conversation IA");

        RuleForEnum(x => x.Role, ["user", "assistant", "system"], "role");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu du message est obligatoire");

        RuleFor(x => x.TokensUtilises)
            .GreaterThanOrEqualTo(0).WithMessage("Le nombre de tokens ne peut pas etre negatif")
            .When(x => x.TokensUtilises.HasValue);
    }
}
