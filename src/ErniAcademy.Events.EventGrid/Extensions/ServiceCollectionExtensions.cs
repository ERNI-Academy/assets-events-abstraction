using Azure.Core;
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
    public static IServiceCollection AddEventGridFromKey(this IServiceCollection services,
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

    public static IServiceCollection AddEventGridFromTokenCredential(this IServiceCollection services,
        IConfiguration configuration,        
        ISerializer serializer, 
        string sectionKey,
        TokenCredential tokenCredential)
    {
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
