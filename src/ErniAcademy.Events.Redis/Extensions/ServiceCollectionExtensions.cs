using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.Redis.Configuration;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ErniAcademy.Events.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method to configure IEventPublisher contract with RedisPublisher by default will use connection string options to connect to Redis database
    /// </summary>
    /// <param name="services">the ServiceCollection</param>
    /// <param name="configuration">the Configuration used to bind and configure the options</param>
    /// <param name="serializer">the serializer to be use</param>
    /// <param name="sectionKey">the configuration section key to get the options</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddEventsRedis(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey)
    {
        services.AddOptions<ConnectionStringOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IConnectionMultiplexerProvider, ConnectionMultiplexerProvider>();
        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();
        
        services.TryAddSingleton<IEventPublisher>(provider =>
        {
            var connectionMultiplexerProvider = provider.GetRequiredService<IConnectionMultiplexerProvider>();
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();

            return new RedisPublisher(connectionMultiplexerProvider, serializer, eventNameResolver);
        });

        return services;
    }
}
