using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.EventGrid.ClientProvider;
using ErniAcademy.Events.EventGrid.Configuration;
using ErniAcademy.Serializers.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ErniAcademy.Events.EventGrid.UnitTests.EventGridPublisherTests;

public class BuildEvent
{

    private readonly EventGridPublisher _sut;
    private readonly IEventGridPublisherClientProvider _eventGridPublisherClientProvider;
    private readonly IEventNameResolver _eventNameResolver;
    private readonly ISerializer _serializer;
    private readonly IOptionsMonitor<PublisherOptions> _options;

    public BuildEvent()
    {
        _eventGridPublisherClientProvider = Substitute.For<IEventGridPublisherClientProvider>();
        _eventNameResolver = Substitute.For<IEventNameResolver>();
        _serializer = Substitute.For<ISerializer>();
        _options = Substitute.For<IOptionsMonitor<PublisherOptions>>();

        _sut = new EventGridPublisher(_eventGridPublisherClientProvider, _eventNameResolver, _serializer, _options);
    }

    [Fact]
    public async Task Should_build_an_event_grid_event_from_event()
    {
        //Arrange
        var @event = new DummyEvent { Title = "hello" };
        @event.AddMetadata("key1", "value1");
        var stream = new MemoryStream();

        _serializer.ContentType.Returns("application/test");
        await _serializer.SerializeToStreamAsync<DummyEvent>(Arg.Is<DummyEvent>(s => s == @event), Arg.Any<Stream>());

        _eventNameResolver.Resolve(Arg.Is<DummyEvent>(e => e == @event)).Returns("dummyevent");

        var options = new PublisherOptions { DataVersion = "v5", SubjectBasePath = "/test/path" };
        _options.CurrentValue.Returns(options);

        //Act
        var actual = await _sut.BuildEvent(@event);

        //Assert
        actual.Data.Should().NotBeNull();
        actual.Topic.Should().Be("dummyevent");
        actual.Id.Should().Be(@event.EventId.ToString());
        actual.EventTime.Should().Be(@event.CreatedAt);
        actual.Subject.Should().Be("/test/path/dummyevent");
    }

    private class DummyEvent : EventBase
    {
        public string Title { get; set; }
    }
}