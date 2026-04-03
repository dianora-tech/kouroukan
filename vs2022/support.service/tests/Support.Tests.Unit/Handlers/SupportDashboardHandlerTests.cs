using FluentAssertions;
using Support.Application.Handlers;
using Support.Application.Queries;
using Support.Domain.Entities;
using Support.Domain.Ports.Output;
using Moq;
using Xunit;

namespace Support.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour SupportDashboardHandler.
/// </summary>
public sealed class SupportDashboardHandlerTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<ISuggestionRepository> _suggestionRepoMock;
    private readonly Mock<IArticleAideRepository> _articleRepoMock;
    private readonly Mock<IConversationIARepository> _conversationRepoMock;
    private readonly SupportDashboardHandler _sut;

    public SupportDashboardHandlerTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();
        _suggestionRepoMock = new Mock<ISuggestionRepository>();
        _articleRepoMock = new Mock<IArticleAideRepository>();
        _conversationRepoMock = new Mock<IConversationIARepository>();

        _sut = new SupportDashboardHandler(
            _ticketRepoMock.Object,
            _suggestionRepoMock.Object,
            _articleRepoMock.Object,
            _conversationRepoMock.Object);
    }

    // ─── Dashboard complet ───

    [Fact]
    public async Task Handle_RetourneDashboardComplet()
    {
        _ticketRepoMock.Setup(r => r.CountByStatutAsync("Ouvert", It.IsAny<CancellationToken>())).ReturnsAsync(5);
        _ticketRepoMock.Setup(r => r.CountByStatutAsync("EnCours", It.IsAny<CancellationToken>())).ReturnsAsync(3);
        _ticketRepoMock.Setup(r => r.CountByStatutAsync("EnAttente", It.IsAny<CancellationToken>())).ReturnsAsync(2);
        _ticketRepoMock.Setup(r => r.CountByStatutAsync("Resolu", It.IsAny<CancellationToken>())).ReturnsAsync(10);
        _ticketRepoMock.Setup(r => r.CountByStatutAsync("Ferme", It.IsAny<CancellationToken>())).ReturnsAsync(20);
        _ticketRepoMock.Setup(r => r.GetTempsMoyenResolutionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(4.5);
        _ticketRepoMock.Setup(r => r.GetMoyenneSatisfactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(4.2);

        _suggestionRepoMock
            .Setup(r => r.GetTopVoteesAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Suggestion>
            {
                new() { Id = 1, Titre = "Top suggestion", NombreVotes = 50 },
            });

        _articleRepoMock
            .Setup(r => r.GetTopConsultesAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>
            {
                new() { Id = 1, Titre = "Top article", NombreVues = 100 },
            });

        _conversationRepoMock
            .Setup(r => r.GetTotalConversationsActivesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(15);

        _conversationRepoMock
            .Setup(r => r.GetTotalTokensConsommesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(50000L);

        var result = await _sut.Handle(new GetSupportDashboardQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.TicketsOuverts.Should().Be(5);
        result.TicketsEnCours.Should().Be(3);
        result.TicketsEnAttente.Should().Be(2);
        result.TicketsResolus.Should().Be(10);
        result.TicketsFermes.Should().Be(20);
        result.TempsMoyenResolutionHeures.Should().Be(4.5);
        result.TauxSatisfactionMoyen.Should().Be(4.2);
        result.TopSuggestions.Should().HaveCount(1);
        result.TopSuggestions[0].Titre.Should().Be("Top suggestion");
        result.TopSuggestions[0].NombreVotes.Should().Be(50);
        result.TopArticles.Should().HaveCount(1);
        result.TopArticles[0].Titre.Should().Be("Top article");
        result.TopArticles[0].NombreVues.Should().Be(100);
        result.ConversationsIAActives.Should().Be(15);
        result.TokensConsommes.Should().Be(50000L);
    }

    // ─── Dashboard vide ───

    [Fact]
    public async Task Handle_RetourneDashboardVide_QuandAucuneDonnee()
    {
        _ticketRepoMock.Setup(r => r.CountByStatutAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _ticketRepoMock.Setup(r => r.GetTempsMoyenResolutionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0.0);
        _ticketRepoMock.Setup(r => r.GetMoyenneSatisfactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0.0);

        _suggestionRepoMock
            .Setup(r => r.GetTopVoteesAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Suggestion>());

        _articleRepoMock
            .Setup(r => r.GetTopConsultesAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>());

        _conversationRepoMock
            .Setup(r => r.GetTotalConversationsActivesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _conversationRepoMock
            .Setup(r => r.GetTotalTokensConsommesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0L);

        var result = await _sut.Handle(new GetSupportDashboardQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.TicketsOuverts.Should().Be(0);
        result.TicketsEnCours.Should().Be(0);
        result.TopSuggestions.Should().BeEmpty();
        result.TopArticles.Should().BeEmpty();
        result.ConversationsIAActives.Should().Be(0);
        result.TokensConsommes.Should().Be(0L);
    }

    // ─── Verification des appels ───

    [Fact]
    public async Task Handle_AppelleTousLesRepositories()
    {
        _ticketRepoMock.Setup(r => r.CountByStatutAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _ticketRepoMock.Setup(r => r.GetTempsMoyenResolutionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0.0);
        _ticketRepoMock.Setup(r => r.GetMoyenneSatisfactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0.0);
        _suggestionRepoMock.Setup(r => r.GetTopVoteesAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Suggestion>());
        _articleRepoMock.Setup(r => r.GetTopConsultesAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ArticleAide>());
        _conversationRepoMock.Setup(r => r.GetTotalConversationsActivesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _conversationRepoMock.Setup(r => r.GetTotalTokensConsommesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0L);

        await _sut.Handle(new GetSupportDashboardQuery(), CancellationToken.None);

        _ticketRepoMock.Verify(r => r.CountByStatutAsync("Ouvert", It.IsAny<CancellationToken>()), Times.Once);
        _ticketRepoMock.Verify(r => r.CountByStatutAsync("EnCours", It.IsAny<CancellationToken>()), Times.Once);
        _ticketRepoMock.Verify(r => r.CountByStatutAsync("EnAttente", It.IsAny<CancellationToken>()), Times.Once);
        _ticketRepoMock.Verify(r => r.CountByStatutAsync("Resolu", It.IsAny<CancellationToken>()), Times.Once);
        _ticketRepoMock.Verify(r => r.CountByStatutAsync("Ferme", It.IsAny<CancellationToken>()), Times.Once);
        _ticketRepoMock.Verify(r => r.GetTempsMoyenResolutionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _ticketRepoMock.Verify(r => r.GetMoyenneSatisfactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _suggestionRepoMock.Verify(r => r.GetTopVoteesAsync(10, It.IsAny<CancellationToken>()), Times.Once);
        _articleRepoMock.Verify(r => r.GetTopConsultesAsync(10, It.IsAny<CancellationToken>()), Times.Once);
        _conversationRepoMock.Verify(r => r.GetTotalConversationsActivesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _conversationRepoMock.Verify(r => r.GetTotalTokensConsommesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
