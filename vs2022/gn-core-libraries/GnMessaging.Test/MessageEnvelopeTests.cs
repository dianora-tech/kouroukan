using System.Text.Json;
using FluentAssertions;
using GnMessaging.Events;
using GnMessaging.Models;

namespace GnMessaging.Test;

public class MessageEnvelopeTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Act
        var envelope = new MessageEnvelope();

        // Assert
        envelope.MessageId.Should().NotBeEmpty();
        envelope.CorrelationId.Should().NotBeEmpty();
        envelope.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        envelope.Source.Should().BeEmpty();
        envelope.Type.Should().BeEmpty();
        envelope.Payload.Should().BeEmpty();
        envelope.RetryCount.Should().Be(0);
    }

    [Fact]
    public void Envelope_ShouldSerializeAndDeserialize()
    {
        // Arrange
        var @event = new EntityCreatedEvent<TestEntity>(new TestEntity { Id = 42, Name = "Test" }, userId: 1);
        var envelope = new MessageEnvelope
        {
            MessageId = @event.MessageId,
            CorrelationId = Guid.NewGuid(),
            Source = "test-service",
            Type = typeof(EntityCreatedEvent<TestEntity>).FullName!,
            Payload = JsonSerializer.Serialize(@event)
        };

        // Act
        var json = JsonSerializer.Serialize(envelope);
        var deserialized = JsonSerializer.Deserialize<MessageEnvelope>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.MessageId.Should().Be(envelope.MessageId);
        deserialized.CorrelationId.Should().Be(envelope.CorrelationId);
        deserialized.Source.Should().Be("test-service");
        deserialized.Type.Should().Contain("EntityCreatedEvent");
    }

    [Fact]
    public void Envelope_ShouldDeserializePayload()
    {
        // Arrange
        var entity = new TestEntity { Id = 42, Name = "Ecole" };
        var @event = new EntityCreatedEvent<TestEntity>(entity);
        var envelope = new MessageEnvelope
        {
            Payload = JsonSerializer.Serialize(@event)
        };

        // Act
        var deserialized = JsonSerializer.Deserialize<EntityCreatedEvent<TestEntity>>(envelope.Payload);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Data.Should().NotBeNull();
        deserialized.Data!.Id.Should().Be(42);
        deserialized.Data.Name.Should().Be("Ecole");
    }

    [Fact]
    public void MessageId_ShouldBeUniquePerInstance()
    {
        // Arrange & Act
        var envelope1 = new MessageEnvelope();
        var envelope2 = new MessageEnvelope();

        // Assert
        envelope1.MessageId.Should().NotBe(envelope2.MessageId);
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
