using FluentAssertions;
using GnValidation.Rules;

namespace GnValidation.Test.Rules;

/// <summary>
/// Tests unitaires pour <see cref="StringSanitizer"/>.
/// </summary>
public sealed class StringSanitizerTests
{
    private readonly StringSanitizer _sanitizer = new();

    [Fact]
    public void StripHtml_WithScriptTag_ShouldRemoveTag()
    {
        // Arrange
        const string input = "<script>alert('xss')</script>Hello";

        // Act
        var result = _sanitizer.StripHtml(input);

        // Assert
        result.Should().Be("alert('xss')Hello", "les balises HTML doivent etre supprimees");
    }

    [Fact]
    public void StripHtml_WithMultipleTags_ShouldRemoveAllTags()
    {
        // Arrange
        const string input = "<p>Bonjour <strong>monde</strong></p>";

        // Act
        var result = _sanitizer.StripHtml(input);

        // Assert
        result.Should().Be("Bonjour monde", "toutes les balises HTML doivent etre supprimees");
    }

    [Fact]
    public void StripHtml_WithoutTags_ShouldReturnSameString()
    {
        // Arrange
        const string input = "Texte simple sans HTML";

        // Act
        var result = _sanitizer.StripHtml(input);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void SanitizeForSql_WithInjection_ShouldEscape()
    {
        // Arrange
        const string input = "Robert'; DROP TABLE eleves--";

        // Act
        var result = _sanitizer.SanitizeForSql(input);

        // Assert
        result.Should().NotContain("';", "les guillemets simples suivis de point-virgule doivent etre neutralises");
        result.Should().NotContain("--", "les commentaires SQL doivent etre supprimes");
    }

    [Fact]
    public void SanitizeForSql_SingleQuote_ShouldBeDoubled()
    {
        // Arrange
        const string input = "l'ecole";

        // Act
        var result = _sanitizer.SanitizeForSql(input);

        // Assert
        result.Should().Be("l''ecole", "les guillemets simples doivent etre doubles");
    }

    [Fact]
    public void NormalizeWhitespace_MultipleSpaces_ShouldNormalize()
    {
        // Arrange
        const string input = "Bonjour    monde   ici";

        // Act
        var result = _sanitizer.NormalizeWhitespace(input);

        // Assert
        result.Should().Be("Bonjour monde ici", "les espaces multiples doivent etre normalises");
    }

    [Fact]
    public void NormalizeWhitespace_TabsAndNewlines_ShouldNormalize()
    {
        // Arrange
        const string input = "Bonjour\t\tmonde\n\nici";

        // Act
        var result = _sanitizer.NormalizeWhitespace(input);

        // Assert
        result.Should().Be("Bonjour monde ici", "les tabulations et retours a la ligne doivent etre normalises");
    }

    [Fact]
    public void Sanitize_CombinedInput_ShouldClean()
    {
        // Arrange
        const string input = "  <b>Bonjour</b>    monde  ";

        // Act
        var result = _sanitizer.Sanitize(input);

        // Assert
        result.Should().Be("Bonjour monde", "la combinaison strip HTML + normalize + trim doit fonctionner");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Sanitize_EmptyOrNull_ShouldReturnEmpty(string? input)
    {
        // Act
        var result = _sanitizer.Sanitize(input!);

        // Assert
        result.Should().BeEmpty();
    }
}
