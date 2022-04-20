using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.Extensions;
using ErniAcademy.Serializers.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace ErniAcademy.Events.ServiceBus.UnitTests.ServiceCollectionExtensionsTests;

public class AddEventsSubscriberTopicServiceBus
{
    private readonly ISerializer _serializer;

    public AddEventsSubscriberTopicServiceBus()
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

        string subscriptionName = "test";

        var services = new ServiceCollection();
        services.AddEventsSubscriberTopicServiceBus<DummyEvent>(configuration, _serializer, "ServiceBus", subscriptionName);
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
        string subscriptionName = "test";

        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>(key, value)
                }).Build();

        var services = new ServiceCollection();
        services.AddEventsSubscriberTopicServiceBus<DummyEvent>(configuration, _serializer, "ServiceBus", subscriptionName);
        var provider = services.BuildServiceProvider();

        //Act
        var actual = () => provider.GetRequiredService<IEventSubscriber<DummyEvent>>();

        //Assert
        var error = actual.Should().Throw<OptionsValidationException>();
        error.Which.Message.Should().Contain("DataAnnotation validation failed for 'ConnectionStringOptions'");
    }

    [Fact]
    public void With_null_subscriptionName_options_section_Throws_ArgumentNullException()
    {
        //Arrange
        string subscriptionName = null;

        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("ServiceBus:ConnectionString", "Endpoint=sb://mock.servicebus.windows.net/;SharedAccessKeyName=mock;SharedAccessKey=MOCK+iWHbYV80ToXyikgi9eGJpQg7Hb7hOj+ejl1Zgjs="),
                }).Build();

        var services = new ServiceCollection();

        var provider = services.BuildServiceProvider();

        //Act
        var actual = () => services.AddEventsSubscriberTopicServiceBus<DummyEvent>(configuration, _serializer, "ServiceBus", subscriptionName); ;

        //Assert
        var error = actual.Should().Throw<ArgumentNullException>();
        error.Which.Message.Should().Contain("subscriptionName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void With_invalid_subscriptionName_options_section_Throws_ArgumentException(string subscriptionName)
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("ServiceBus:ConnectionString", "Endpoint=sb://mock.servicebus.windows.net/;SharedAccessKeyName=mock;SharedAccessKey=MOCK+iWHbYV80ToXyikgi9eGJpQg7Hb7hOj+ejl1Zgjs="),
                }).Build();

        var services = new ServiceCollection();
        
        var provider = services.BuildServiceProvider();

        //Act
        var actual = () => services.AddEventsSubscriberTopicServiceBus<DummyEvent>(configuration, _serializer, "ServiceBus", subscriptionName); ;

        //Assert
        var error = actual.Should().Throw<ArgumentException>();
        error.Which.Message.Should().Contain("subscriptionName");
    }

    private class DummyEvent : EventBase { }
}