using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.StorageQueues.Extensions;
using ErniAcademy.Serializers.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace ErniAcademy.Events.StorageQueues.UnitTests.ServiceCollectionExtensionsTests;

public class AddEventsSubscriberStorageQueues
{
    private readonly ISerializer _serializer;

    public AddEventsSubscriberStorageQueues()
    {
        _serializer = Substitute.For<ISerializer>();
    }

    [Fact]
    public void With_valid_options_section_Should_configure_IEventSubscriber_with_StorageQueuesPublisher_impl()
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("StorageQueues:ConnectionString", "UseDevelopmentStorage=true"),
                }).Build();

        var services = new ServiceCollection();
        services.AddEventsSubscriberStorageQueues<DummyEvent>(configuration, _serializer, "StorageQueues");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = provider.GetRequiredService<IEventSubscriber<DummyEvent>>();

        //Assert
        actual.Should().NotBeNull();
    }

    private class DummyEvent : EventBase { }
}