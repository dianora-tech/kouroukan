using FluentAssertions;
using GnDapper.Models;
using Communication.Application.Handlers;
using Communication.Application.Queries;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Communication.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour MessageQueryHandler.
/// </summary>
public sealed class MessageQueryHandlerTests
{
    private readonly Mock<IMessageService> _serviceMock;
    private readonly MessageQueryHandler _sut;

    public MessageQueryHandlerTests()
    {
        _serviceMock = new Mock<IMessageService>();
        _sut = new MessageQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneMessage()
    {
        var message = new Message { Id = 1, Sujet = "Test" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);

        var result = await _sut.Handle(new GetMessageByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Message?)null);

        var result = await _sut.Handle(new GetMessageByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var messages = new List<Message>
        {
            new() { Id = 1, Sujet = "Test 1" },
            new() { Id = 2, Sujet = "Test 2" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        var result = await _sut.Handle(new GetAllMessagesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Message>(
            new List<Message> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedMessagesQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
