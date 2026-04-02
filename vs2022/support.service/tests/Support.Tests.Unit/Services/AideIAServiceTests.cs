using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Support.Domain.Entities;
using Support.Domain.Ports.Output;
using Support.Domain.Services;
using Xunit;

namespace Support.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour AideIAService — mock Ollama via IOllamaClient.
/// </summary>
public sealed class AideIAServiceTests
{
    private readonly Mock<IOllamaClient> _ollamaMock;
    private readonly Mock<IArticleAideRepository> _articleRepoMock;
    private readonly Mock<IConversationIARepository> _conversationRepoMock;
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<ILogger<AideIAService>> _loggerMock;
    private readonly AideIAService _sut;

    public AideIAServiceTests()
    {
        _ollamaMock = new Mock<IOllamaClient>();
        _articleRepoMock = new Mock<IArticleAideRepository>();
        _conversationRepoMock = new Mock<IConversationIARepository>();
        _ticketRepoMock = new Mock<ITicketRepository>();
        _loggerMock = new Mock<ILogger<AideIAService>>();

        _sut = new AideIAService(
            _conversationRepoMock.Object,
            _articleRepoMock.Object,
            _ticketRepoMock.Object,
            _ollamaMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GenererReponseAsync_RetourneReponseFormatee()
    {
        var conversation = new ConversationIA { Id = 1, UtilisateurId = 42, UserId = 1, EstActive = true };
        _conversationRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _conversationRepoMock.Setup(r => r.CountMessagesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _articleRepoMock.Setup(r => r.RechercherFullTextAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>
            {
                new() { Id = 1, Titre = "Guide inscriptions", Contenu = "Pour inscrire un eleve..." }
            });

        _ollamaMock.Setup(o => o.IsAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ollamaMock.Setup(o => o.ChatAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<(string, string)>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("Pour inscrire un eleve, allez dans le module Inscriptions.", 42));

        _conversationRepoMock.Setup(r => r.GetMessagesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MessageIA>());
        _conversationRepoMock.Setup(r => r.AddMessageAsync(It.IsAny<MessageIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MessageIA { Id = 1 });
        _conversationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<ConversationIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.GenererReponseAsync(1, "Comment inscrire un eleve ?");

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("inscrire");
    }

    [Fact]
    public async Task GenererReponseAsync_InjecteArticlesContexteRAG()
    {
        var conversation = new ConversationIA { Id = 1, UtilisateurId = 42, UserId = 1, EstActive = true };
        _conversationRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _conversationRepoMock.Setup(r => r.CountMessagesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _articleRepoMock.Setup(r => r.RechercherFullTextAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>
            {
                new() { Id = 1, Titre = "Inscriptions", Contenu = "Guide..." }
            });

        _ollamaMock.Setup(o => o.IsAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ollamaMock.Setup(o => o.ChatAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<(string, string)>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("Reponse", 10));

        _conversationRepoMock.Setup(r => r.GetMessagesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MessageIA>());
        _conversationRepoMock.Setup(r => r.AddMessageAsync(It.IsAny<MessageIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MessageIA { Id = 1 });
        _conversationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<ConversationIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.GenererReponseAsync(1, "inscription");

        _ollamaMock.Verify(o => o.ChatAsync(
            It.Is<string>(prompt => prompt.Contains("Guide")),
            It.IsAny<IReadOnlyList<(string, string)>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenererReponseAsync_RetourneMessageErreur_SiOllamaTimeout()
    {
        var conversation = new ConversationIA { Id = 1, UtilisateurId = 42, UserId = 1, EstActive = true };
        _conversationRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _conversationRepoMock.Setup(r => r.CountMessagesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _articleRepoMock.Setup(r => r.RechercherFullTextAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>());

        _ollamaMock.Setup(o => o.IsAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ollamaMock.Setup(o => o.ChatAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<(string, string)>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TaskCanceledException("Timeout"));

        _conversationRepoMock.Setup(r => r.GetMessagesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MessageIA>());
        _conversationRepoMock.Setup(r => r.AddMessageAsync(It.IsAny<MessageIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MessageIA { Id = 1 });
        _conversationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<ConversationIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.GenererReponseAsync(1, "question");

        result.Should().Contain("indisponible");
    }

    [Fact]
    public async Task GenererReponseAsync_RetourneMessageErreur_SiOllamaDown()
    {
        var conversation = new ConversationIA { Id = 1, UtilisateurId = 42, UserId = 1, EstActive = true };
        _conversationRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _conversationRepoMock.Setup(r => r.CountMessagesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _articleRepoMock.Setup(r => r.RechercherFullTextAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>());

        _ollamaMock.Setup(o => o.IsAvailableAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _conversationRepoMock.Setup(r => r.AddMessageAsync(It.IsAny<MessageIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MessageIA { Id = 1 });
        _conversationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<ConversationIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.GenererReponseAsync(1, "question");

        result.Should().NotBeNullOrEmpty();
    }
}
