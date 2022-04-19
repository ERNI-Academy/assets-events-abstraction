using Azure.Core;
using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.ClientProvider;
using ErniAcademy.Events.ServiceBus.Configuration;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.ServiceBus.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceBusFromConnectionString(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey,
        ServiceBusClientOptions busOptions = null)
    {
        services.AddOptions<ConnectionStringOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventPublisher>(provider =>
        {
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
            var serviceBusClientProvider = new ConnectionStringProvider(
                provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>(),
                busOptions);

            return new ServiceBusPublisher(serviceBusClientProvider, eventNameResolver, serializer);
        });

        return services;
    }

    public static IServiceCollection AdderviceBusFromTokenCredential(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey,
        TokenCredential tokenCredential,
        ServiceBusClientOptions busOptions = null)
    {
        services.AddOptions<FullyQualifiedNamespaceOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventPublisher>(provider =>
        {
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
            var serviceBusClientProvider = new TokenCredentialProvider(
                provider.GetRequiredService<IOptionsMonitor<FullyQualifiedNamespaceOptions>>(),
                tokenCredential,
                busOptions);

            return new ServiceBusPublisher(serviceBusClientProvider, eventNameResolver, serializer);
        });

        return services;
    }
}
