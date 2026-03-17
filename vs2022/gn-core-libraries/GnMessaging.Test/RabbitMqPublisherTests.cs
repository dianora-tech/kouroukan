using GnMessaging.Events;
using GnMessaging.Models;
using GnMessaging.RabbitMq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace GnMessaging.Test;

public class RabbitMqPublisherTests
{
    private readonly Mock<RabbitMqConnectionManager> _connectionManagerMock;
    private readonly Mock<IModel> _channelMock;
    private readonly RabbitMqPublisher _publisher;

    public RabbitMqPublisherTests()
    {
        var options = Options.Create(new RabbitMqOptions
        {
            ProjectSlug = "kouroukan",
            ServiceName = "test-service"
        });

        _connectionManagerMock = new Mock<RabbitMqConnectionManager>(
            MockBehavior.Loose,
            Options.Create(new RabbitMqOptions()),
            new Mock<ILogger<RabbitMqConnectionManager>>().Object);

        _channelMock = new Mock<IModel>();

        var propsMock = new Mock<IBasicProperties>();
        propsMock.SetupAllProperties();
        _channelMock.Setup(c => c.CreateBasicProperties()).Returns(propsMock.Object);

        _connectionManagerMock.Setup(cm => cm.CreateChannel()).Returns(_channelMock.Object);

        var logger = new Mock<ILogger<RabbitMqPublisher>>();
        _publisher = new RabbitMqPublisher(_connectionManagerMock.Object, options, logger.Object);
    }

    [Fact]
    public async Task PublishAsync_ShouldThrowOnNullMessage()
    {
        var act = () => _publisher.PublishAsync<EntityCreatedEvent<TestEntity>>(
            null!, "exchange", "routing.key");

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PublishAsync_ShouldThrowOnEmptyExchange()
    {
        var message = new EntityCreatedEvent<TestEntity>(new TestEntity());

        var act = () => _publisher.PublishAsync(message, "", "routing.key");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task PublishAsync_ShouldThrowOnEmptyRoutingKey()
    {
        var message = new EntityCreatedEvent<TestEntity>(new TestEntity());

        var act = () => _publisher.PublishAsync(message, "exchange", "");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task PublishAsync_ShouldDeclareExchangeAndPublish()
    {
        var message = new EntityCreatedEvent<TestEntity>(new TestEntity { Id = 1, Name = "Test" });
        var exchange = "kouroukan.events";
        var routingKey = "entity.created.testentity";

        await _publisher.PublishAsync(message, exchange, routingKey);

        _channelMock.Verify(c => c.ExchangeDeclare(
            exchange, ExchangeType.Topic, true, false, null), Times.Once);

        _channelMock.Verify(c => c.BasicPublish(
            exchange, routingKey, false,
            It.IsAny<IBasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
    }

    [Fact]
    public void PublishOptions_ShouldHaveCorrectDefaults()
    {
        var options = new PublishOptions();

        options.Persistent.Should().BeTrue();
        options.Priority.Should().Be(0);
        options.Exchange.Should().BeNull();
        options.RoutingKey.Should().BeNull();
        options.CorrelationId.Should().BeNull();
    }

    [Fact]
    public void ConsumeOptions_ShouldHaveCorrectDefaults()
    {
        var options = new ConsumeOptions();

        options.PrefetchCount.Should().Be(10);
        options.AutoAck.Should().BeFalse();
        options.RetryCount.Should().Be(5);
        options.BindingKeys.Should().ContainSingle().Which.Should().Be("#");
    }

    [Fact]
    public void RabbitMqOptions_ShouldBuildDefaultExchange()
    {
        var options = new RabbitMqOptions { ProjectSlug = "kouroukan" };

        options.DefaultExchange.Should().Be("kouroukan.events");
    }

    [Fact]
    public void EntityCreatedEvent_ShouldSetEntityType()
    {
        var @event = new EntityCreatedEvent<TestEntity>(new TestEntity());

        @event.EntityType.Should().Be("TestEntity");
        @event.MessageId.Should().NotBeEmpty();
        @event.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void EntityUpdatedEvent_ShouldSetEntityType()
    {
        var @event = new EntityUpdatedEvent<TestEntity>(new TestEntity(), userId: 5);

        @event.EntityType.Should().Be("TestEntity");
        @event.UserId.Should().Be(5);
    }

    [Fact]
    public void EntityDeletedEvent_ShouldSetEntityIdAndType()
    {
        var @event = new EntityDeletedEvent<TestEntity>(42, userId: 1);

        @event.EntityId.Should().Be(42);
        @event.EntityType.Should().Be("TestEntity");
        @event.UserId.Should().Be(1);
    }

    [Fact]
    public void CacheInvalidatedEvent_ShouldSetCacheKey()
    {
        var @event = new CacheInvalidatedEvent("niveaux-classes", "Entity updated");

        @event.CacheKey.Should().Be("niveaux-classes");
        @event.Reason.Should().Be("Entity updated");
        @event.MessageId.Should().NotBeEmpty();
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
