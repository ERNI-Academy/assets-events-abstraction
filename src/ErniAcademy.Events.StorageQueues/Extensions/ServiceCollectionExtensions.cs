using Azure.Storage.Queues;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.StorageQueues.ClientProvider;
using ErniAcademy.Events.StorageQueues.Configuration;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.StorageQueues.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method to configure IEventPublisher contract with StorageQueuePublisher by default will use connection string options to connect to Storage Queue
    /// </summary>
    /// <param name="services">the ServiceCollection</param>
    /// <param name="configuration">the Configuration used to bind and configure the options</param>
    /// <param name="serializer">the serializer to be use</param>
    /// <param name="sectionKey">the configuration section key to get the options</param>
    /// <param name="queueOptions">the QueueClient options for extra configuration</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddEventsStorageQueues(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey,
        QueueClientOptions queueOptions = null)
    {
        services.AddOptions<ConnectionStringOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventPublisher>(provider =>
        {
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
            var queueClientProvider = new ConnectionStringProvider(provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>(), queueOptions);

            return new StorageQueuePublisher(queueClientProvider, eventNameResolver, serializer);
        });

        return services;
    }

    public static IServiceCollection AddEventsSubscriberStorageQueues<TEvent>(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey,
        QueueClientOptions queueOptions = null)
        where TEvent : class, IEvent, new()
    {
        services.AddOptions<ConnectionStringOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventSubscriber<TEvent>>(provider =>
        {
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
            var queueClientProvider = new ConnectionStringProvider(provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>(), queueOptions);

            return new StorageQueueSubscriber<TEvent>(queueClientProvider, eventNameResolver, serializer);
        });

        return services;
    }
}
