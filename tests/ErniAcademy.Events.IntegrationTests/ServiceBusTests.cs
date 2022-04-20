using ErniAcademy.Events.ServiceBus.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErniAcademy.Events.IntegrationTests;

public class ServiceBusTests : BaseTests
{
    protected override IServiceCollection RegisterSut(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEventsPublisherServiceBus(configuration, _serializer, sectionKey: "Events:ServiceBus");
        services.AddEventsSubscriberTopicServiceBus<DummyEvent>(configuration, _serializer, sectionKey: "Events:ServiceBus", subscriptionName: "testprocessor");
        return services;
    }
}
