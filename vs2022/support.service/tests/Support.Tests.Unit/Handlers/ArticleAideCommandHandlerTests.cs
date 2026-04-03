using FluentAssertions;
using Support.Application.Commands;
using Support.Application.Handlers;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Support.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour ArticleAideCommandHandler.
/// </summary>
public sealed class ArticleAideCommandHandlerTests
{
    private readonly Mock<IArticleAideService> _serviceMock;
    private readonly ArticleAideCommandHandler _sut;

    public ArticleAideCommandHandlerTests()
    {
        _serviceMock = new Mock<IArticleAideService>();
        _sut = new ArticleAideCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneArticle()
    {
        var command = new CreateArticleAideCommand(
            Name: "Article test",
            Description: "Description",
            TypeId: 1,
            Titre: "Comment se connecter",
            Contenu: "Pour vous connecter, suivez ces etapes...",
            Slug: "comment-se-connecter",
            Categorie: "Guide",
            ModuleConcerne: "Auth",
            Ordre: 1,
            EstPublie: true,
            UserId: 1);

        var expected = new ArticleAide
        {
            Id = 42,
            Name = "Article test",
            Titre = "Comment se connecter",
            Slug = "comment-se-connecter",
            Categorie = "Guide",
            EstPublie = true
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<ArticleAide>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<ArticleAide>(a =>
                a.Name == "Article test" &&
                a.Titre == "Comment se connecter" &&
                a.Contenu == "Pour vous connecter, suivez ces etapes..." &&
                a.Slug == "comment-se-connecter" &&
                a.Categorie == "Guide" &&
                a.ModuleConcerne == "Auth" &&
                a.Ordre == 1 &&
                a.EstPublie == true &&
                a.UserId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateArticleAideCommand(
            Id: 1,
            Name: "Article modifie",
            Description: "Desc",
            TypeId: 1,
            Titre: "Titre modifie",
            Contenu: "Contenu modifie",
            Slug: "titre-modifie",
            Categorie: "FAQ",
            ModuleConcerne: "Dashboard",
            Ordre: 2,
            EstPublie: false,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<ArticleAide>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<ArticleAide>(a =>
                a.Id == 1 &&
                a.Name == "Article modifie" &&
                a.Slug == "titre-modifie" &&
                a.EstPublie == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateArticleAideCommand(
            Id: 999, Name: "Test", Description: null, TypeId: 1,
            Titre: "Test", Contenu: "Test", Slug: "test",
            Categorie: "Guide", ModuleConcerne: null, Ordre: 0,
            EstPublie: true, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<ArticleAide>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteArticleAideCommand(1);

        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistant()
    {
        var command = new DeleteArticleAideCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── MarquerUtile ───

    [Fact]
    public async Task Handle_MarquerUtileCommand_AppelleService_EtRetourneTrue()
    {
        var command = new MarquerArticleUtileCommand(1);

        _serviceMock
            .Setup(s => s.MarquerUtileAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.MarquerUtileAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MarquerUtileCommand_RetourneFalse_SiInexistant()
    {
        var command = new MarquerArticleUtileCommand(999);

        _serviceMock
            .Setup(s => s.MarquerUtileAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
