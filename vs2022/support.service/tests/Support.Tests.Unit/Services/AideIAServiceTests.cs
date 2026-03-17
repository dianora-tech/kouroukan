using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Support.Domain.Entities;
using Support.Domain.Ports.Output;
using Support.Domain.Services;
using Support.Infrastructure.ExternalServices;
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
    private readonly Mock<IMessageIARepository> _messageRepoMock;
    private readonly Mock<ILogger<AideIAService>> _loggerMock;
    private readonly AideIAService _sut;

    public AideIAServiceTests()
    {
        _ollamaMock = new Mock<IOllamaClient>();
        _articleRepoMock = new Mock<IArticleAideRepository>();
        _conversationRepoMock = new Mock<IConversationIARepository>();
        _messageRepoMock = new Mock<IMessageIARepository>();
        _loggerMock = new Mock<ILogger<AideIAService>>();

        _sut = new AideIAService(
            _ollamaMock.Object,
            _articleRepoMock.Object,
            _conversationRepoMock.Object,
            _messageRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SendMessageAsync_RetourneReponseFormatee()
    {
        var conversation = new ConversationIA { Id = 1, UtilisateurId = 42, EstActive = true };
        _conversationRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        _articleRepoMock.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>
            {
                new() { Id = 1, Titre = "Guide inscriptions", Contenu = "Pour inscrire un eleve..." }
            });

        _ollamaMock.Setup(o => o.GenerateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Pour inscrire un eleve, allez dans le module Inscriptions.");

        _messageRepoMock.Setup(r => r.AddAsync(It.IsAny<MessageIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MessageIA { Id = 1 });

        var result = await _sut.SendMessageAsync(1, "Comment inscrire un eleve ?");

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("inscrire");
    }

    [Fact]
    public async Task SendMessageAsync_InjecteArticlesContexteRAG()
    {
        var conversation = new ConversationIA { Id = 1, UtilisateurId = 42, EstActive = true };
        _conversationRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        _articleRepoMock.Setup(r => r.SearchAsync("inscription", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>
            {
                new() { Id = 1, Titre = "Inscriptions", Contenu = "Guide..." }
            });

        _ollamaMock.Setup(o => o.GenerateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Reponse");
        _messageRepoMock.Setup(r => r.AddAsync(It.IsAny<MessageIA>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MessageIA { Id = 1 });

        await _sut.SendMessageAsync(1, "inscription");

        _ollamaMock.Verify(o => o.GenerateAsync(
            It.Is<string>(prompt => prompt.Contains("Guide")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_RetourneMessageErreur_SiOllamaTimeout()
    {
        var conversation = new ConversationIA { Id = 1, UtilisateurId = 42, EstActive = true };
        _conversationRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        _articleRepoMock.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>());

        _ollamaMock.Setup(o => o.GenerateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TaskCanceledException("Timeout"));

        var result = await _sut.SendMessageAsync(1, "question");

        result.Should().Contain("indisponible").Or.Contain("erreur").Or.Contain("reessayer");
    }

    [Fact]
    public async Task SendMessageAsync_RetourneMessageErreur_SiOllamaDown()
    {
        var conversation = new ConversationIA { Id = 1, UtilisateurId = 42, EstActive = true };
        _conversationRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        _articleRepoMock.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ArticleAide>());

        _ollamaMock.Setup(o => o.GenerateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var result = await _sut.SendMessageAsync(1, "question");

        result.Should().NotBeNullOrEmpty();
    }
}
