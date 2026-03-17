using FluentAssertions;
using FluentValidation.TestHelper;
using GnValidation.Commands;
using GnValidation.FluentValidation;
using GnValidation.Options;
using GnValidation.Rules;

namespace GnValidation.Test.FluentValidation;

/// <summary>
/// Tests unitaires pour <see cref="CreateUserValidator"/>.
/// </summary>
public sealed class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator;

    public CreateUserValidatorTests()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new GnValidationOptions());
        var phoneValidator = new PhoneNumberValidator(options);
        var passwordValidator = new PasswordStrengthValidator(options);
        var emailValidator = new EmailValidator();
        _validator = new CreateUserValidator(phoneValidator, passwordValidator, emailValidator);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new CreateUserCommand(
            "Mamadou",
            "Diallo",
            "mamadou@kouroukan.gn",
            "+224621000000",
            "MonPass@123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyFirstName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateUserCommand(
            "",
            "Diallo",
            "mamadou@kouroukan.gn",
            "+224621000000",
            "MonPass@123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_ShortFirstName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateUserCommand(
            "A",
            "Diallo",
            "mamadou@kouroukan.gn",
            "+224621000000",
            "MonPass@123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("Le prenom doit contenir au moins 2 caracteres");
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = new CreateUserCommand(
            "Mamadou",
            "Diallo",
            "not-an-email",
            "+224621000000",
            "MonPass@123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_InvalidPhone_ShouldHaveError()
    {
        // Arrange
        var command = new CreateUserCommand(
            "Mamadou",
            "Diallo",
            "mamadou@kouroukan.gn",
            "abc123",
            "MonPass@123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_WeakPassword_ShouldHaveError()
    {
        // Arrange
        var command = new CreateUserCommand(
            "Mamadou",
            "Diallo",
            "mamadou@kouroukan.gn",
            "+224621000000",
            "weak");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_EmptyPassword_ShouldHaveError()
    {
        // Arrange
        var command = new CreateUserCommand(
            "Mamadou",
            "Diallo",
            "mamadou@kouroukan.gn",
            "+224621000000",
            "");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
