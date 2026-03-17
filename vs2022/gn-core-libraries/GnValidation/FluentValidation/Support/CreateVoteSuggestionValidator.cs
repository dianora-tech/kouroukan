using GnValidation.Commands.Support;

namespace GnValidation.FluentValidation.Support;

/// <summary>
/// Validateur pour la creation d'un vote sur une suggestion.
/// Verifie la suggestion et le votant.
/// </summary>
public sealed class CreateVoteSuggestionValidator : BaseEntityValidator<CreateVoteSuggestionCommand>
{
    public CreateVoteSuggestionValidator()
    {
        RuleForRequiredFk(x => x.SuggestionId, "suggestion");

        RuleForRequiredFk(x => x.VotantId, "votant");
    }
}
