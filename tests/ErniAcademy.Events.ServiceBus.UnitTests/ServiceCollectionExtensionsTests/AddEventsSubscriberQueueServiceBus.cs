using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.Extensions;
using ErniAcademy.Serializers.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace ErniAcademy.Events.ServiceBus.UnitTests.ServiceCollectionExtensionsTests;

public class AddEventsSubscriberQueueServiceBus
{
    private readonly ISerializer _serializer;

    public AddEventsSubscriberQueueServiceBus()
    {
        _serializer = Substitute.For<ISerializer>();
    }

    [Fact]
    public void With_valid_options_section_Should_configure_IEventPublisher_with_ServiceBusPublisher_impl()
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("ServiceBus:ConnectionString", "Endpoint=sb://mock.servicebus.windows.net/;SharedAccessKeyName=mock;SharedAccessKey=MOCK+iWHbYV80ToXyikgi9eGJpQg7Hb7hOj+ejl1Zgjs="),
                }).Build();

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddDebug());
        services.AddEventsSubscriberQueueServiceBus<DummyEvent>(configuration, _serializer, "ServiceBus");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = provider.GetRequiredService<IEventSubscriber<DummyEvent>>();

        //Assert
        actual.Should().NotBeNull();
    }

    [Theory]
    [InlineData("ServiceBus:ConnectionString", "")]
    [InlineData("ServiceBus:ConnectionString", "  ")]
    [InlineData("ServiceBus:ConnectionString", null)]
    public void With_wrong_connectionstring_options_section_Throws_OptionsValidationException(string key, string value)
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>(key, value)
                }).Build();

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddDebug());
        services.AddEventsSubscriberQueueServiceBus<DummyEvent>(configuration, _serializer, "ServiceBus");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = () => provider.GetRequiredService<IEventSubscriber<DummyEvent>>();

        //Assert
        var error = actual.Should().Throw<OptionsValidationException>();
        error.Which.Message.Should().Contain("DataAnnotation validation failed for 'ConnectionStringOptions'");
    }

    private class DummyEvent : EventBase { }
}