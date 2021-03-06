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
    /// Extension method to configure IEventPublisher contract with RedisPublisher by default will use connection string options 
    /// to connect to Redis database, make sure ConnectionString of Redis is configure in the configuration section.
    /// </summary>
    /// <param name="services">the ServiceCollection</param>
    /// <param name="configuration">the Configuration used to bind and configure the options</param>
    /// <param name="serializer">the serializer to be use</param>
    /// <param name="sectionKey">the configuration section key to get the options</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddEventsPublisherRedis(this IServiceCollection services,
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

    /// <summary>
    /// Extension method to configure IEventSubscriber<TEvent> contract with RedisSubscriber<TEvent> by default will use connection string options
    /// to connect to Redis database, make sure ConnectionString of Redis is configure in the configuration section.
    /// </summary>
    /// <typeparam name="TEvent">The generic event type to be subscribed</typeparam>
    /// <param name="services">the ServiceCollection</param>
    /// <param name="configuration">the Configuration used to bind and configure the options</param>
    /// <param name="serializer">the serializer to be use</param>
    /// <param name="sectionKey">the configuration section key to get the options</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddEventsSubscriberRedis<TEvent>(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey)
        where TEvent : class, IEvent, new()
    {
        services.AddOptions<ConnectionStringOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IConnectionMultiplexerProvider, ConnectionMultiplexerProvider>();
        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventSubscriber<TEvent>>(provider =>
        {
            var connectionMultiplexerProvider = provider.GetRequiredService<IConnectionMultiplexerProvider>();
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();

            return new RedisSubscriber<TEvent>(connectionMultiplexerProvider, serializer, eventNameResolver);
        });

        return services;
    }
}
