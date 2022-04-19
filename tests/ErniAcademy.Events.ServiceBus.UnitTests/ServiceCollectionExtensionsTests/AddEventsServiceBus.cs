using Azure.Core;
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

public class AddEventsServiceBus
{
    private readonly ISerializer _serializer;

    public AddEventsServiceBus()
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
        services.AddEventsServiceBus(configuration, _serializer, "ServiceBus");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = provider.GetRequiredService<IEventPublisher>();

        //Assert
        actual.Should().NotBeNull();
    }

    [Fact]
    public void With_valid_tokenCredentials_options_section_Should_configure_IEventPublisher_with_ServiceBusPublisher_impl()
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("ServiceBus:FullyQualifiedNamespace", "mock.servicebus.windows.net"),
                }).Build();

        TokenCredential tokenCredential = Substitute.For<TokenCredential>();

        var services = new ServiceCollection();
        services.AddEventsServiceBus(configuration, _serializer, "ServiceBus", tokenCredential);
        var provider = services.BuildServiceProvider();

        //Act
        var actual = provider.GetRequiredService<IEventPublisher>();

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
        services.AddEventsServiceBus(configuration, _serializer, "ServiceBus");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = () => provider.GetRequiredService<IEventPublisher>();

        //Assert
        var error = actual.Should().Throw<OptionsValidationException>();
        error.Which.Message.Should().Contain("DataAnnotation validation failed for 'ConnectionStringOptions'");
    }

    [Fact]
    public void With_null_tokenCredential_section_Throws_ArgumentNullException()
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("ServiceBus:Key", "testkey")
                }).Build();

        TokenCredential tokenCredential = null;

        var services = new ServiceCollection();

        //Act
        var actual = () => services.AddEventsServiceBus(configuration, _serializer, "ServiceBus", tokenCredential);

        //Assert
        var error = actual.Should().Throw<ArgumentNullException>();
        error.Which.Message.Should().Contain("Value cannot be null. (Parameter 'tokenCredential')");
    }

    [Theory]
    [InlineData("ServiceBus:FullyQualifiedNamespace", "")]
    [InlineData("ServiceBus:FullyQualifiedNamespace", "  ")]
    [InlineData("ServiceBus:FullyQualifiedNamespace", null)]
    public void With_wrong_fullyqualifiednamespace_options_section_Throws_OptionsValidationException(string key, string value)
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>(key, value)
                }).Build();

        TokenCredential tokenCredential = Substitute.For<TokenCredential>();

        var services = new ServiceCollection();
        services.AddEventsServiceBus(configuration, _serializer, "ServiceBus", tokenCredential);
        var provider = services.BuildServiceProvider();

        //Act
        var actual = () => provider.GetRequiredService<IEventPublisher>();

        //Assert
        var error = actual.Should().Throw<OptionsValidationException>();
        error.Which.Message.Should().Contain("DataAnnotation validation failed for 'FullyQualifiedNamespaceOptions'");
    }
}