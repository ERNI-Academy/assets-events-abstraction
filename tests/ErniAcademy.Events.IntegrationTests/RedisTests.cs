using ErniAcademy.Events.Redis.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErniAcademy.Events.IntegrationTests;

public class RedisTests : BaseTests
{
    protected override IServiceCollection RegisterSut(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEventsRedis(configuration, _serializer, sectionKey: "Events:Redis");
        services.AddEventsSubscriberRedis<DummyEvent>(configuration, _serializer, sectionKey: "Events:Redis");
        return services;
    }
}
