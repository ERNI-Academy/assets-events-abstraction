using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Azure.Storage.Queues;
using ErniAcademy.Events.EventGrid.Extensions;
using ErniAcademy.Serializers.Contracts;
using ErniAcademy.Serializers.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.IntegrationTests;

public class EventGridTests : BaseTests
{
    private readonly QueueClient _processor;
    private readonly ISerializer _serializer = new JsonSerializer();

    public EventGridTests()
    {
        var options = _provider.GetRequiredService<IOptionsMonitor<QueueOptions>>();
        _processor = new QueueClient(options.CurrentValue.ConnectionString, "testprocessor");
        _processor.ClearMessages();
    }

    protected override IServiceCollection RegisterSut(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEventsEventGrid(configuration, _serializer, sectionKey: "Events:EventGrid");

        services.AddOptions<QueueOptions>().Bind(configuration.GetSection("Events:EventGrid")).ValidateDataAnnotations();

        return services;
    }

    protected override async Task<DummyEvent> WaitForReceive()
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
