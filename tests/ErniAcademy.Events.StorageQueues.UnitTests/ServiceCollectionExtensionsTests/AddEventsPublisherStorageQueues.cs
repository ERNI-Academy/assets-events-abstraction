using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.StorageQueues.Extensions;
using ErniAcademy.Serializers.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ErniAcademy.Events.StorageQueues.UnitTests.ServiceCollectionExtensionsTests;

public class AddEventsPublisherStorageQueues
{
    private readonly ISerializer _serializer;

    public AddEventsPublisherStorageQueues()
    {
        _serializer = Substitute.For<ISerializer>();
    }

    [Fact]
    public void With_valid_options_section_Should_configure_IEventPublisher_with_StorageQueuesPublisher_impl()
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>("StorageQueues:ConnectionString", "UseDevelopmentStorage=true"),
                }).Build();

        var services = new ServiceCollection();
        services.AddEventsPublisherStorageQueues(configuration, _serializer, "StorageQueues");
        var provider = services.BuildServiceProvider();

        //Act
        var actual = provider.GetRequiredService<IEventPublisher>();

        //Assert
        actual.Should().NotBeNull();
    }

    [Theory]
    [InlineData("StorageQueues:ConnectionString", "")]
    [InlineData("StorageQueues:ConnectionString", "  ")]
    [InlineData("StorageQueues:ConnectionString", null)]
    public async Task With_wrong_connectionstring_options_section_Throws_OptionsValidationException(string key, string value)
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[]{
                    new KeyValuePair<string, string>(key, value)
                }).Build();

        var services = new ServiceCollection();
        services.AddEventsPublisherStorageQueues(configuration, _serializer, "StorageQueues");
        var provider = services.BuildServiceProvider();

        var publisher = provider.GetRequiredService<IEventPublisher>();

        //Act
        var actual = ()=> publisher.PublishAsync<DummyEvent>(new DummyEvent());

        //Assert
        var error = await actual.Should().ThrowAsync<OptionsValidationException>();
        error.Which.Message.Should().Contain("DataAnnotation validation failed for 'ConnectionStringOptions'");
    }

    private class DummyEvent : EventBase { }
}