using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GnValidation.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace GnValidation.Test.Behaviors;

/// <summary>
/// Tests unitaires pour <see cref="ValidationPipelineBehavior{TRequest, TResponse}"/>.
/// </summary>
public sealed class ValidationPipelineBehaviorTests
{
    // --- Types de test internes ---
    public record TestRequest(string Name) : IRequest<TestResponse>;
    public record TestResponse(bool Success);

    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var logger = new Mock<ILogger<ValidationPipelineBehavior<TestRequest, TestResponse>>>();
        var behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(validators, logger.Object);

        var request = new TestRequest("test");
        var expectedResponse = new TestResponse(true);

        // Act
        var result = await behavior.Handle(
            request,
            () => Task.FromResult(expectedResponse),
            CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse, "le handler doit etre appele quand il n'y a pas de validateurs");
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCallNext()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validators = new[] { validatorMock.Object };
        var logger = new Mock<ILogger<ValidationPipelineBehavior<TestRequest, TestResponse>>>();
        var behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(validators, logger.Object);

        var request = new TestRequest("test");
        var expectedResponse = new TestResponse(true);

        // Act
        var result = await behavior.Handle(
            request,
            () => Task.FromResult(expectedResponse),
            CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse, "le handler doit etre appele quand la validation reussit");
    }

    [Fact]
    public async Task Handle_InvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Name", "Le nom est obligatoire")
        };

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var validators = new[] { validatorMock.Object };
        var logger = new Mock<ILogger<ValidationPipelineBehavior<TestRequest, TestResponse>>>();
        var behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(validators, logger.Object);

        var request = new TestRequest("");

        // Act
        var act = () => behavior.Handle(
            request,
            () => Task.FromResult(new TestResponse(true)),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>("une requete invalide doit lever une exception")
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Name"));
    }

    [Fact]
    public async Task Handle_MultipleValidators_ShouldAggregateErrors()
    {
        // Arrange
        var validator1Mock = new Mock<IValidator<TestRequest>>();
        validator1Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Name", "Erreur 1") }));

        var validator2Mock = new Mock<IValidator<TestRequest>>();
        validator2Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Name", "Erreur 2") }));

        var validators = new[] { validator1Mock.Object, validator2Mock.Object };
        var logger = new Mock<ILogger<ValidationPipelineBehavior<TestRequest, TestResponse>>>();
        var behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(validators, logger.Object);

        var request = new TestRequest("");

        // Act
        var act = () => behavior.Handle(
            request,
            () => Task.FromResult(new TestResponse(true)),
            CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2, "les erreurs de tous les validateurs doivent etre agregees");
    }
}
