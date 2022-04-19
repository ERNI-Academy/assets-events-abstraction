using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using ErniAcademy.Events.StorageQueues.Configuration;
using ErniAcademy.Events.StorageQueues.Extensions;
using ErniAcademy.Serializers.Contracts;
using ErniAcademy.Serializers.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.IntegrationTests;

public class StorageQueueTests : BaseTests
{
    private readonly QueueClient _processor;
    private readonly ISerializer _serializer = new JsonSerializer();

    public StorageQueueTests()
    {
        var options = _provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>();
        _processor = new QueueClient(options.CurrentValue.ConnectionString, "dummyevent");
        _processor.ClearMessages();
    }

    protected override IServiceCollection RegisterSut(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEventsStorageQueues(configuration, _serializer, sectionKey: "Events:StorageQueues");
        return services;
    }

    protected override async Task<DummyEvent> WaitForReceive()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));

        var message = (await _processor.ReceiveMessageAsync(TimeSpan.FromSeconds(5))).Value;

        await _processor.DeleteMessageAsync(message.MessageId, message.PopReceipt);

        var result = _serializer.DeserializeFromString<DummyEvent>(message.Body.ToString());

        return result;
    }
}
