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
            var queueClientProvider = new ConnectionStringProvider(
                provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>(),
                queueOptions);

            return new StorageQueuePublisher(queueClientProvider, eventNameResolver, serializer);
        });

        return services;
    }
}
