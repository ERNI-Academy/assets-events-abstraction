using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.ServiceBus.Configuration;
using ErniAcademy.Events.ServiceBus.Extensions;
using ErniAcademy.Events.ServiceBus.Serializers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.IntegrationTests;

public class ServiceBusTests : BaseTests
{
    private readonly ServiceBusProcessor _processor;
    private readonly ISerializer _serializer = new JsonSerializer();

    public ServiceBusTests()
    {
        var options = _provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>();
        ServiceBusClient client = new ServiceBusClient(options.CurrentValue.ConnectionString);
        _processor = client.CreateProcessor("dummyevent", "testprocessor", new ServiceBusProcessorOptions());
    }

    protected override IServiceCollection RegisterSut(IServiceCollection services)
    {
        services.AddErniAcademyConnectionStringServiceBus(_serializer);
        return services;
    }

    protected override async Task<DummyEvent> WaitForReceive()
    {
        DummyEvent result = default(DummyEvent);

        _processor.ProcessMessageAsync += async args =>
        {
            var body = args.Message.Body.ToString();
            await args.CompleteMessageAsync(args.Message);
            result = _serializer.DeserializeFromString<DummyEvent>(body);
        };

        _processor.ProcessErrorAsync += args => { throw args.Exception; };

        await _processor.StartProcessingAsync();

        await Task.Delay(TimeSpan.FromSeconds(5));

        await _processor.StopProcessingAsync();
        await _processor.DisposeAsync();

        return result;
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
