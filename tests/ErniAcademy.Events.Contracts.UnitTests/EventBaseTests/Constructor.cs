using ErniAcademy.Events.Contracts.UnitTests.Utils;
using FluentAssertions;
using System;
using Xunit;

namespace ErniAcademy.Events.Contracts.UnitTests.EventBaseTests;

public partial class Constructor
{
    [Fact]
    public void With_default_ctor()
    {
        //Act
        var sut = new DummyEvent();

        //Assert
        sut.EventId.Should().NotBeEmpty();
        sut.CorrelationId.Should().NotBeEmpty();
        sut.EventType.Should().Be(typeof(DummyEvent).FullName);
        sut.CreatedAt.Should().NotBe(DateTimeOffset.MinValue);
        sut.Metadata.Should().NotBeNull();
        sut.Metadata.Values.Should().BeEmpty();
    }

    [Fact]
    public void With_correlationid_ctor()
    {
        //Arrange
        var correlationId = Guid.NewGuid();

        //Act
        var sut = new DummyEvent(correlationId);

        //Assert
        sut.EventId.Should().NotBeEmpty();
        sut.CorrelationId.Should().Be(correlationId);
        sut.EventType.Should().Be(typeof(DummyEvent).FullName);
        sut.CreatedAt.Should().NotBe(System.DateTimeOffset.MinValue);
        sut.Metadata.Should().NotBeNull();
        sut.Metadata.Values.Should().BeEmpty();
    }

    [Fact]
    public void With_eventtype_ctor()
    {
        //Arrange
        var eventType = "custom event type";

        //Act
        var sut = new DummyEvent(eventType);

        //Assert
        sut.EventId.Should().NotBeEmpty();
        sut.CorrelationId.Should().NotBeEmpty();
        sut.EventType.Should().Be(eventType);
        sut.CreatedAt.Should().NotBe(System.DateTimeOffset.MinValue);
        sut.Metadata.Should().NotBeNull();
        sut.Metadata.Values.Should().BeEmpty();
    }

    [Fact]
    public void With_full_ctor()
    {
        //Arrange
        var eventId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var eventType = "custom event type";
        var createdAt = DateTimeOffset.UtcNow;

        //Act
        var sut = new DummyEvent(eventId, correlationId, eventType, createdAt);

        //Assert
        sut.EventId.Should().Be(eventId);
        sut.CorrelationId.Should().Be(correlationId);
        sut.EventType.Should().Be(eventType);
        sut.CreatedAt.Should().Be(createdAt);
        sut.Metadata.Should().NotBeNull();
        sut.Metadata.Values.Should().BeEmpty();
    }
}
