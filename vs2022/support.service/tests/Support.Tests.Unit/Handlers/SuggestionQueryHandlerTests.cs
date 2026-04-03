using FluentAssertions;
using GnDapper.Models;
using Support.Application.Handlers;
using Support.Application.Queries;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Support.Domain.Ports.Output;
using Moq;
using Xunit;

namespace Support.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour SuggestionQueryHandler.
/// </summary>
public sealed class SuggestionQueryHandlerTests
{
    private readonly Mock<ISuggestionService> _serviceMock;
    private readonly Mock<ISuggestionRepository> _repositoryMock;
    private readonly SuggestionQueryHandler _sut;

    public SuggestionQueryHandlerTests()
    {
        _serviceMock = new Mock<ISuggestionService>();
        _repositoryMock = new Mock<ISuggestionRepository>();
        _sut = new SuggestionQueryHandler(_serviceMock.Object, _repositoryMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetById_RetourneSuggestion()
    {
        var suggestion = new Suggestion { Id = 1, Titre = "Test" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(suggestion);

        var result = await _sut.Handle(new GetSuggestionByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Suggestion?)null);

        var result = await _sut.Handle(new GetSuggestionByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var suggestions = new List<Suggestion>
        {
            new() { Id = 1, Titre = "Suggestion 1" },
            new() { Id = 2, Titre = "Suggestion 2" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(suggestions);

        var result = await _sut.Handle(new GetAllSuggestionsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListeVide()
    {
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Suggestion>());

        var result = await _sut.Handle(new GetAllSuggestionsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Suggestion>(
            new List<Suggestion> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedSuggestionsQuery(1, 20, "test", null, "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── GetTopSuggestions ───

    [Fact]
    public async Task Handle_GetTopSuggestions_AppelleRepository()
    {
        var suggestions = new List<Suggestion>
        {
            new() { Id = 1, Titre = "Top 1", NombreVotes = 50 },
            new() { Id = 2, Titre = "Top 2", NombreVotes = 30 },
        };
        _repositoryMock
            .Setup(r => r.GetTopVoteesAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(suggestions);

        var result = await _sut.Handle(new GetTopSuggestionsQuery(5), CancellationToken.None);

        result.Should().HaveCount(2);
        _repositoryMock.Verify(r => r.GetTopVoteesAsync(5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GetTopSuggestions_RetourneListeVide()
    {
        _repositoryMock
            .Setup(r => r.GetTopVoteesAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Suggestion>());

        var result = await _sut.Handle(new GetTopSuggestionsQuery(10), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
