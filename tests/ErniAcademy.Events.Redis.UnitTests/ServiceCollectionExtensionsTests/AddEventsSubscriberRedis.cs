using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.Redis.Extensions;
using ErniAcademy.Serializers.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace ErniAcademy.Events.Redis.UnitTests.ServiceCollectionExtensionsTests;

public class AddEventsSubscriberRedis
{
    private readonly ISerializer _serializer;

    public AddEventsSubscriberRedis()
    {
        _serializer = Substitute.For<ISerializer>();
    }

    [Fact]
    public void With_valid_options_section_Should_configure_IEventSubscriber()
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("Redis:ConnectionString", "localhost")
                }).Build();

        var services = new ServiceCollection();
        services.AddEventsSubscriberRedis<DummyEvent>(configuration, _serializer, "Redis");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = provider.GetRequiredService<IEventSubscriber<DummyEvent>>();

        //Assert
        actual.Should().NotBeNull();
    }

    private class DummyEvent : EventBase { }
}