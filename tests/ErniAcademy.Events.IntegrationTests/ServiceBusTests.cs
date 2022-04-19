using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.ServiceBus.Configuration;
using ErniAcademy.Events.ServiceBus.Extensions;
using ErniAcademy.Serializers.Contracts;
using ErniAcademy.Serializers.Json;
using Microsoft.Extensions.Configuration;
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

    protected override IServiceCollection RegisterSut(IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceBusFromConnectionString(configuration, _serializer, sectionKey: "Events:ServiceBus");
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
}
