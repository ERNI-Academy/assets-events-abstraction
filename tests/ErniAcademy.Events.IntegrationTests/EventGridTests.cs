using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Azure.Storage.Queues;
using ErniAcademy.Events.EventGrid.Extensions;
using ErniAcademy.Events.EventGrid.Serializers;
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

    protected override IServiceCollection RegisterSut(IServiceCollection services)
    {
        services.AddEventGridFromKey(_serializer, sectionKey: "Events:EventGrid");
        services.ConfigureOptions<QueueOptions>(sectionKey: "Events:EventGrid");
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
        public string ConnectionString { get; set; }
    }


    /// <summary>
    /// TODO: Omar. Replace this with https://github.com/ERNI-Academy/assets-serializers-abstraction ISerializer
    /// </summary>
    private class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public JsonSerializer(JsonSerializerOptions jsonSerializerOptions = null)
        {
            _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions();
        }

        public string ContentType => "application/json";

        public void SerializeToStream<TItem>(TItem item, Stream stream) => SerializeToStreamAsync(item, stream).GetAwaiter().GetResult();

        public async Task SerializeToStreamAsync<TItem>(TItem item, Stream stream, CancellationToken cancellationToken = default)
        {
            await System.Text.Json.JsonSerializer.SerializeAsync<TItem>(stream, item, _jsonSerializerOptions, cancellationToken);
            stream.Position = 0;
        }

        public string SerializeToString<TItem>(TItem item) => System.Text.Json.JsonSerializer.Serialize<TItem>(item, _jsonSerializerOptions);

        public TItem DeserializeFromStream<TItem>(Stream stream) => System.Text.Json.JsonSerializer.Deserialize<TItem>(stream, _jsonSerializerOptions);

        public TItem DeserializeFromString<TItem>(string item) => System.Text.Json.JsonSerializer.Deserialize<TItem>(item, _jsonSerializerOptions);

        public ValueTask<TItem> DeserializeFromStreamAsync<TItem>(Stream stream, CancellationToken cancellationToken = default) => System.Text.Json.JsonSerializer.DeserializeAsync<TItem>(stream, _jsonSerializerOptions, cancellationToken);
    }
}
