using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.ClientProvider;
using ErniAcademy.Serializers.Contracts;
using FluentAssertions;
using NSubstitute;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ErniAcademy.Events.ServiceBus.UnitTests.ServiceBusPublisherTests;

public class BuildMessage
{
    private readonly ServiceBusPublisher _sut;
    private readonly IServiceBusClientProvider _serviceBusClientProvider;
    private readonly IEventNameResolver _eventNameResolver;
    private readonly ISerializer _serializer;

    public BuildMessage()
    {
        _serviceBusClientProvider = Substitute.For<IServiceBusClientProvider>(); 
        _eventNameResolver = Substitute.For<IEventNameResolver>();
        _serializer = Substitute.For<ISerializer>();

        _sut = new ServiceBusPublisher(_serviceBusClientProvider, _eventNameResolver, _serializer);
    }

    [Fact]
    public async Task Should_build_a_service_bus_message_from_event()
    {
        //Arrange
        var @event = new DummyEvent { Title = "hello" };
        @event.AddMetadata("key1", "value1");
        var stream = new MemoryStream();

        _serializer.ContentType.Returns("application/test");
        await _serializer.SerializeToStreamAsync<DummyEvent>(Arg.Is<DummyEvent>(s => s == @event), Arg.Any<Stream>());

        //Act
        var actual = await _sut.BuildMessage(@event);

        //Assert
        actual.ContentType.Should().Be("application/test");
        actual.Body.Should().NotBeNull();
        actual.ApplicationProperties.Should().NotBeNullOrEmpty().And.ContainSingle(x=>x.Key == "key1" && x.Value.ToString() == "value1");
    }

    private class DummyEvent : EventBase
    {
        public string Title { get; set; }
    }
}
