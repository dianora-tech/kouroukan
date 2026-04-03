using Xunit;
using Support.Api.Controllers;
using Support.Api.Models;
using Support.Application.Commands;
using Support.Application.Queries;
using Support.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Support.Tests.Unit.Controllers;

public sealed class ConversationIAControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<ConversationIAController>> _logger;
    private readonly ConversationIAController _sut;

    public ConversationIAControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<ConversationIAController>>();
        _sut = new ConversationIAController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerOk_QuandConversationCreee()
    {
        var conversation = new ConversationIA() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<CreerConversationIACommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var command = new CreerConversationIACommand(1, 1);
        var result = await _sut.Create(command, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    // ─── EnvoyerMessage ───

    [Fact]
    public async Task EnvoyerMessage_DoitRetournerOk_QuandMessageEnvoye()
    {
        _mediator.Setup(m => m.Send(It.IsAny<EnvoyerMessageIACommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("reponse IA");

        var request = new EnvoyerMessageRequest("question", 1);
        var result = await _sut.EnvoyerMessage(1, request, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    // ─── GetMessages ───

    [Fact]
    public async Task GetMessages_DoitRetournerOk_QuandAppele()
    {
        var messages = new List<MessageIA>() as IReadOnlyList<MessageIA>;
        _mediator.Setup(m => m.Send(It.IsAny<GetMessagesConversationIAQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        var result = await _sut.GetMessages(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
