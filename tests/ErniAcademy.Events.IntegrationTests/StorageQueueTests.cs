using ErniAcademy.Events.StorageQueues.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErniAcademy.Events.IntegrationTests;

public class StorageQueueTests : BaseTests
{
    protected override IServiceCollection RegisterSut(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEventsStorageQueues(configuration, _serializer, sectionKey: "Events:StorageQueues");
        services.AddEventsSubscriberStorageQueues<DummyEvent>(configuration, _serializer, sectionKey: "Events:StorageQueues");
        return services;
    }
}
