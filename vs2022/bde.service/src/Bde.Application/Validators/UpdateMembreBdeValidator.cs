using FluentValidation;
using GnValidation.FluentValidation;
using Bde.Application.Commands;

namespace Bde.Application.Validators;

public sealed class UpdateMembreBdeValidator : BaseEntityValidator<UpdateMembreBdeCommand>
{
    public UpdateMembreBdeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("L'identifiant est obligatoire");
        RuleForRequiredString(x => x.Name, 200, "Nom");
        RuleForOptionalString(x => x.Description, 1000, "Description");
        RuleForRequiredFk(x => x.AssociationId, "Association");
        RuleForRequiredFk(x => x.EleveId, "Eleve");
        RuleForEnum(x => x.RoleBde, ["President", "Tresorier", "Secretaire", "RespPole", "Membre"], "Role BDE");
        RuleFor(x => x.DateAdhesion).NotEmpty().WithMessage("La date d'adhesion est obligatoire");
        RuleForRequiredFk(x => x.UserId, "Utilisateur");
    }
}
