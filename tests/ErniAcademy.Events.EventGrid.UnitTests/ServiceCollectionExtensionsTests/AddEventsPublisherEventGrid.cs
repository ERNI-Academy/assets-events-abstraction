using Azure.Core;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.EventGrid.Extensions;
using ErniAcademy.Serializers.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace ErniAcademy.Events.EventGrid.UnitTests.ServiceCollectionExtensionsTests;

public class AddEventsPublisherEventGrid
{
    private readonly ISerializer _serializer;

    public AddEventsPublisherEventGrid()
    {
        _serializer = Substitute.For<ISerializer>();
    }


    [Fact]
    public void With_valid_options_section_Should_configure_IEventPublisher_with_EventGridPublisher_impl()
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("EventGrid:Endpoint", "https://test.eventgrid.com/"),
                    new KeyValuePair<string, string>("EventGrid:Key", "testkey")
                }).Build();

        var services = new ServiceCollection();
        services.AddEventsPublisherEventGrid(configuration, _serializer, "EventGrid");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = provider.GetRequiredService<IEventPublisher>();

        //Assert
        actual.Should().NotBeNull();
    }

    [Theory]
    [InlineData("EventGrid:Key", "")]
    [InlineData("EventGrid:Key", "  ")]
    [InlineData("EventGrid:Key", null)]
    public void With_wrong_key_options_section_Throws_OptionsValidationException(string key, string value)
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("EventGrid:Endpoint", "https://test.eventgrid.com/"),
                    new KeyValuePair<string, string>(key, value)
                }).Build();

        var services = new ServiceCollection();
        services.AddEventsPublisherEventGrid(configuration, _serializer, "EventGrid");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = () => provider.GetRequiredService<IEventPublisher>();

        //Assert
        var error = actual.Should().Throw<OptionsValidationException>();
        error.Which.Message.Should().Contain("DataAnnotation validation failed for 'KeyOptions'");
    }

    [Theory]
    [InlineData("EventGrid:Endpoint", "")]
    [InlineData("EventGrid:Endpoint", null)]
    public void With_wrong_topic_options_section_Throws_OptionsValidationException(string key, string value)
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("EventGrid:Key", "testkey"),
                    new KeyValuePair<string, string>(key, value)
                }).Build();

        var services = new ServiceCollection();
        services.AddEventsPublisherEventGrid(configuration, _serializer, "EventGrid");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = () => provider.GetRequiredService<IEventPublisher>();

        //Assert
        var error = actual.Should().Throw<OptionsValidationException>();
        error.Which.Message.Should().Contain("DataAnnotation validation failed for 'TopicOptions'");
    }

    [Theory]
    [InlineData("EventGrid:Endpoint", "  ")]
    [InlineData("EventGrid:Endpoint", "not a url")]
    public void With_invalid_topic_options_section_Throws_InvalidOperationException(string key, string value)
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("EventGrid:Key", "testkey"),
                    new KeyValuePair<string, string>(key, value)
                }).Build();

        var services = new ServiceCollection();
        services.AddEventsPublisherEventGrid(configuration, _serializer, "EventGrid");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = () => provider.GetRequiredService<IEventPublisher>();

        //Assert
        var error = actual.Should().Throw<InvalidOperationException>();
        error.Which.Message.Should().Contain("This operation is not supported for a relative URI");
    }
}