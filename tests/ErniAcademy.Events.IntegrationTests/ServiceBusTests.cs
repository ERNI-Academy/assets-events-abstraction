using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.Contracts;
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
    private readonly IEventSubscriber<DummyEvent> _subscriber;
    private readonly ISerializer _serializer = new JsonSerializer();

    public ServiceBusTests()
    {
        var options = _provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>();
        ServiceBusClient client = new ServiceBusClient(options.CurrentValue.ConnectionString);
        _subscriber = _provider.GetRequiredService<IEventSubscriber<DummyEvent>>();
    }

    protected override IServiceCollection RegisterSut(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEventsServiceBus(configuration, _serializer, sectionKey: "Events:ServiceBus");
        services.AddEventsSubscriberTopicServiceBus<DummyEvent>(configuration, _serializer, sectionKey: "Events:ServiceBus", subscriptionName: "testprocessor");
        return services;
    }

    protected override async Task<DummyEvent> WaitForReceive()
    {
        DummyEvent result = default(DummyEvent);

        await _subscriber.SubscribeAsync((e) => { result = e; return Task.CompletedTask; });
        await _subscriber.StarProcessingAsync();
        
        await Task.Delay(TimeSpan.FromSeconds(5));

        await _subscriber.StopProcessingAsync();

        return result;
    }
}
