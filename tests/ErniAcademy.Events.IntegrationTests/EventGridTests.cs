using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Azure.Storage.Queues;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.EventGrid.Extensions;
using ErniAcademy.Events.IntegrationTests.Utils;
using ErniAcademy.Serializers.Contracts;
using ErniAcademy.Serializers.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace ErniAcademy.Events.IntegrationTests;

public class EventGridTests
{
    private readonly QueueClient _processor;
    private readonly ISerializer _serializer = new JsonSerializer();
    private readonly IEventPublisher _publisher;

    public EventGridTests()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationHelper.Get();
        services.AddSingleton<IConfiguration>(configuration);

        services.AddEventsEventGrid(configuration, _serializer, sectionKey: "Events:EventGrid");
        services.AddOptions<QueueOptions>().Bind(configuration.GetSection("Events:EventGrid")).ValidateDataAnnotations();

        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptionsMonitor<QueueOptions>>();

        _publisher = provider.GetRequiredService<IEventPublisher>();
        _processor = new QueueClient(options.CurrentValue.ConnectionString, "testprocessor");
        _processor.ClearMessages();
    }

    [Fact]
    public async Task EventGrid_Publish_event_should_be_received_by_a_consumer()
    {
        //Arrange
        var @event = new DummyEvent
        {
            Title = "Integration test event " + Guid.NewGuid()
        };

        //Act
        await _publisher.PublishAsync(@event);

        DummyEvent publishedEvent = await WaitForReceive();

        //Assert
        publishedEvent.Should().BeEquivalentTo(@event);
    }

    [Fact]
    public async Task EventGrid_Publish_events_should_be_received_by_a_consumer()
    {
        //Arrange
        var @events = new[]
        {
            new DummyEvent
            {
                Title = "Integration test event " + Guid.NewGuid()
            }
        };

        //Act
        await _publisher.PublishAsync(@events);

        DummyEvent publishedEvent = await WaitForReceive();

        //Assert
        publishedEvent.Should().BeEquivalentTo(@events[0]);
    }

    protected async Task<DummyEvent> WaitForReceive()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));

        var message = (await _processor.ReceiveMessageAsync( TimeSpan.FromSeconds(5))).Value;

        await _processor.DeleteMessageAsync(message.MessageId, message.PopReceipt);

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(message.Body.ToString()));
        var eventGridEvent = _serializer.DeserializeFromString<EventGridEvent>(json);
        var result = _serializer.DeserializeFromString<DummyEvent>(eventGridEvent.Data.ToString());

        return result;
    }

    private class QueueOptions
    {
        [Required]
        public string ConnectionString { get; set; }
    }
}
