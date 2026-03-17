namespace GnValidation.Commands;

/// <summary>Commande de creation d'un utilisateur.</summary>
public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password);
