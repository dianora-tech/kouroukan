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
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<SuggestionService>> _loggerMock;
    private readonly SuggestionService _sut;

    public SuggestionServiceTests()
    {
        _repoMock = new Mock<ISuggestionRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<SuggestionService>>();

        _sut = new SuggestionService(
            _repoMock.Object,
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
    public async Task VoterAsync_IncrementeCompteur()
    {
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.GetVoteAsync(1, 42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VoteSuggestion?)null);
        _repoMock.Setup(r => r.AddVoteAsync(It.IsAny<VoteSuggestion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VoteSuggestion { Id = 1, SuggestionId = 1, VotantId = 42 });
        _repoMock.Setup(r => r.IncrementVotesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.VoterAsync(1, 42, 1);

        result.Should().BeTrue();
        _repoMock.Verify(r => r.AddVoteAsync(
            It.Is<VoteSuggestion>(v => v.SuggestionId == 1 && v.VotantId == 42),
            It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.IncrementVotesAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task VoterAsync_DeuxFois_LanceException()
    {
        _repoMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.GetVoteAsync(1, 42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VoteSuggestion { Id = 1, SuggestionId = 1, VotantId = 42 });

        var act = async () => await _sut.VoterAsync(1, 42, 1);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*deja vote*");
    }
}
