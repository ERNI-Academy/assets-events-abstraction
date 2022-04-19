﻿using Azure.Core;
using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.ClientProvider;
using ErniAcademy.Events.ServiceBus.Configuration;
using ErniAcademy.Events.ServiceBus.ProcessorProvider;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.ServiceBus.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method to configure IEventPublisher contract with ServiceBusPublisher by default will use connection string options to connect to ServiceBus
    /// </summary>
    /// <param name="services">the ServiceCollection</param>
    /// <param name="configuration">the Configuration used to bind and configure the options</param>
    /// <param name="serializer">the serializer to be use</param>
    /// <param name="sectionKey">the configuration section key to get the options</param>
    /// <param name="busOptions">the ServiceBus options for extra configuration</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddEventsServiceBus(this IServiceCollection services,
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
            var serviceBusClientProvider = new ConnectionStringProvider(provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>(), busOptions);

            return new ServiceBusPublisher(serviceBusClientProvider, eventNameResolver, serializer);
        });

        return services;
    }

    /// <summary>
    /// Extension method to configure IEventPublisher contract with ServiceBusPublisher with TokenCredential options to connect to ServiceBus
    /// </summary>
    /// <param name="services">the ServiceCollection</param>
    /// <param name="configuration">the Configuration used to bind and configure the options</param>
    /// <param name="serializer">the serializer to be use</param>
    /// <param name="sectionKey">the configuration section key to get the options</param>
    /// <param name="tokenCredential">the TokenCredential instance</param>
    /// <param name="busOptions">the ServiceBus options for extra configuration</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddEventsServiceBus(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey,
        TokenCredential tokenCredential,
        ServiceBusClientOptions busOptions = null)
    {
        if (tokenCredential == null)
        {
            throw new ArgumentNullException(nameof(tokenCredential));
        }

        services.AddOptions<FullyQualifiedNamespaceOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventPublisher>(provider =>
        {
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
            var serviceBusClientProvider = new TokenCredentialProvider(provider.GetRequiredService<IOptionsMonitor<FullyQualifiedNamespaceOptions>>(), tokenCredential, busOptions);

            return new ServiceBusPublisher(serviceBusClientProvider, eventNameResolver, serializer);
        });

        return services;
    }

    public static IServiceCollection AddEventsSubscriberQueueServiceBus<TEvent>(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey,
        ServiceBusClientOptions busOptions = null,
        ServiceBusProcessorOptions busProcessorOptions = null)
        where TEvent : class, IEvent, new()
    {
        services.AddOptions<ConnectionStringOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventSubscriber<TEvent>>(provider =>
        {
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
            var serviceBusClientProvider = new ConnectionStringProvider(provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>(), busOptions);
            var serviceBusProcessorProvider = new QueueProvider(serviceBusClientProvider, busProcessorOptions);

            return new ServiceBusSubscriber<TEvent>(serviceBusProcessorProvider, eventNameResolver, serializer);
        });

        return services;
    }

    public static IServiceCollection AddEventsSubscriberTopicServiceBus<TEvent>(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey,
        string subscriptionName,
        ServiceBusClientOptions busOptions = null,
        ServiceBusProcessorOptions busProcessorOptions = null)
        where TEvent : class, IEvent, new()
    {
        services.AddOptions<ConnectionStringOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();
        
        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventSubscriber<TEvent>>(provider =>
        {
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
            var serviceBusClientProvider = new ConnectionStringProvider(provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>(), busOptions);
            var serviceBusProcessorProvider = new TopicProvider(serviceBusClientProvider, subscriptionName, busProcessorOptions);

            return new ServiceBusSubscriber<TEvent>(serviceBusProcessorProvider, eventNameResolver, serializer);
        });

        return services;
    }
}
