﻿using Azure.Core;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.EventGrid.ClientProvider;
using ErniAcademy.Events.EventGrid.Configuration;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.EventGrid.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method to configure IEventPublisher contract with EventGridPublisher by default will use key options to connect to EventGrid
    /// </summary>
    /// <param name="services">the ServiceCollection</param>
    /// <param name="configuration">the Configuration used to bind and configure the options</param>
    /// <param name="serializer">the serializer to be use</param>
    /// <param name="sectionKey">the configuration section key to get the options</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddEventsEventGrid(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey)
    {
        services.AddOptions<TopicOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();
        services.AddOptions<KeyOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();
        services.AddOptions<PublisherOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventPublisher>(provider =>
        {
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
            var eventGridPublisherClientProvider = new KeyCredentialProvider(
                provider.GetRequiredService<IOptionsMonitor<TopicOptions>>(),
                provider.GetRequiredService<IOptionsMonitor<KeyOptions>>());

            var options = provider.GetRequiredService<IOptionsMonitor<PublisherOptions>>();

            return new EventGridPublisher(eventGridPublisherClientProvider, eventNameResolver, serializer, options);
        });

        return services;
    }

    /// <summary>
    /// Extension method to configure IEventPublisher contract with EventGridPublisher with TokenCredential options to connect to EventGrid
    /// </summary>
    /// <param name="services">the ServiceCollection</param>
    /// <param name="configuration">the Configuration used to bind and configure the options</param>
    /// <param name="serializer">the serializer to be use</param>
    /// <param name="sectionKey">the configuration section key to get the options</param>
    /// <param name="tokenCredential">the TokenCredential instance</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddEventsEventGrid(this IServiceCollection services,
        IConfiguration configuration,        
        ISerializer serializer, 
        string sectionKey,
        TokenCredential tokenCredential)
    {
        if (tokenCredential == null)
        {
            throw new ArgumentNullException(nameof(tokenCredential));
        }

        services.AddOptions<TopicOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();
        services.AddOptions<PublisherOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

        services.TryAddSingleton<IEventPublisher>(provider =>
        {
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
            var serviceBusClientProvider = new TokenCredentialProvider(
                provider.GetRequiredService<IOptionsMonitor<TopicOptions>>(),
                tokenCredential);

            var options = provider.GetRequiredService<IOptionsMonitor<PublisherOptions>>();

            return new EventGridPublisher(serviceBusClientProvider, eventNameResolver, serializer, options);
        });

        return services;
    }
}
