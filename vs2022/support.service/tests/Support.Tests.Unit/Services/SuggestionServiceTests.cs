using FluentAssertions;
using GnMessaging.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Support.Domain.Entities;
using Support.Domain.Ports.Output;
using Support.Domain.Services;
using Xunit;

namespace Support.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour SuggestionService — focus sur le systeme de votes.
/// </summary>
public sealed class SuggestionServiceTests
{
    private readonly Mock<ISuggestionRepository> _repoMock;
    private readonly Mock<IVoteSuggestionRepository> _voteRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<SuggestionService>> _loggerMock;
    private readonly SuggestionService _sut;

    public SuggestionServiceTests()
    {
        _repoMock = new Mock<ISuggestionRepository>();
        _voteRepoMock = new Mock<IVoteSuggestionRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<SuggestionService>>();

        _sut = new SuggestionService(
            _repoMock.Object,
            _voteRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_CreeSuggestion_AvecStatutSoumise()
    {
        var suggestion = new Suggestion
        {
            Titre = "Mode sombre",
            Contenu = "Ajouter un mode sombre",
            StatutSuggestion = "Soumise",
            NombreVotes = 0,
            AuteurId = 1,
            UserId = 1
        };

        _repoMock.Setup(r => r.AddAsync(suggestion, It.IsAny<CancellationToken>()))
            .ReturnsAsync(suggestion);

        var result = await _sut.CreateAsync(suggestion);

        result.Should().NotBeNull();
        result.StatutSuggestion.Should().Be("Soumise");
        result.NombreVotes.Should().Be(0);
    }

    [Fact]
    public async Task VoteAsync_IncrementeCompteur()
    {
        var suggestion = new Suggestion { Id = 1, NombreVotes = 5 };

        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(suggestion);
        _voteRepoMock.Setup(r => r.HasVotedAsync(1, 42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _voteRepoMock.Setup(r => r.AddAsync(It.IsAny<VoteSuggestion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VoteSuggestion { Id = 1, SuggestionId = 1, VotantId = 42 });
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Suggestion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.VoteAsync(1, 42);

        _repoMock.Verify(r => r.UpdateAsync(
            It.Is<Suggestion>(s => s.NombreVotes == 6),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task VoteAsync_DeuxFois_LanceException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Suggestion { Id = 1, NombreVotes = 5 });
        _voteRepoMock.Setup(r => r.HasVotedAsync(1, 42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _sut.VoteAsync(1, 42);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*deja vote*");
    }
}
