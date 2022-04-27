using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.IntegrationTests.Utils;
using ErniAcademy.Serializers.Contracts;
using ErniAcademy.Serializers.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ErniAcademy.Events.IntegrationTests;

public abstract class BaseTests
{
    protected IServiceProvider _provider;

    protected IEventPublisher _publisher;
    protected IEventSubscriber<DummyEvent> _subscriber;
    protected ISerializer _serializer = new JsonSerializer();

    protected BaseTests()
    {
        var services = new ServiceCollection();

        var configuration = ConfigurationHelper.Get();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.AddConsole().AddDebug());

        RegisterSut(services, configuration);

        _provider = services.BuildServiceProvider();

        _publisher = _provider.GetService<IEventPublisher>();
        _subscriber = _provider.GetRequiredService<IEventSubscriber<DummyEvent>>();
    }

    protected abstract IServiceCollection RegisterSut(IServiceCollection services, IConfiguration configuration);

    [Fact]
    public virtual async Task Publish_event_should_be_received_by_a_consumer()
    {
        //Arrange
        var @event = new DummyEvent
        {
            Title = "Integration test event " + Guid.NewGuid()
        };

        DummyEvent publishedEvent = null;

        _subscriber.ProcessEventAsync += async e =>
        {
            publishedEvent = e;
            await Task.CompletedTask;
        };

        await _subscriber.StartProcessingAsync();

        //Act
        await _publisher.PublishAsync(@event);

        await Task.Delay(TimeSpan.FromSeconds(5));

        await _subscriber.StopProcessingAsync();

        //Assert
        publishedEvent.Should().BeEquivalentTo(@event);
    }

    [Fact]
    public virtual async Task Publish_events_should_be_received_by_a_consumer()
    {
        //Arrange
        var @events = new[]
        {
            new DummyEvent
            {
                Title = "Integration test event 1" + Guid.NewGuid()
            },
            new DummyEvent
            {
                Title = "Integration test event 2" + Guid.NewGuid()
            },
            new DummyEvent
            {
                Title = "Integration test event 3" + Guid.NewGuid()
            }
        };

        var publishedEvents = new List<DummyEvent>();

        _subscriber.ProcessEventAsync += async e =>
        {
            publishedEvents.Add(e);
            await Task.CompletedTask;
        };

        await _subscriber.StartProcessingAsync();

        //Act
        await _publisher.PublishAsync(@events);

        await Task.Delay(TimeSpan.FromSeconds(5));

        await _subscriber.StopProcessingAsync();

        //Assert
        publishedEvents.Should().BeEquivalentTo(@events);
    }
}
