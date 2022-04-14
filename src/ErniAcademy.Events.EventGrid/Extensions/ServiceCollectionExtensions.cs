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
        ISerializer serializer,
        string sectionKey)
    {
        services.AddOptions();
        services.ConfigureOptions<TopicOptions>(sectionKey);
        services.ConfigureOptions<KeyOptions>(sectionKey);
        services.ConfigureOptions<PublisherOptions>(sectionKey);

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
        TokenCredential tokenCredential,
        ISerializer serializer, 
        string sectionKey)
    {
        services.AddOptions();
        services.ConfigureOptions<TopicOptions>(sectionKey);
        services.ConfigureOptions<PublisherOptions>(sectionKey);

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

    internal static IServiceCollection ConfigureOptions<TOptions>(this IServiceCollection services, string sectionKey)
         where TOptions : class, new()
    {
        services.AddSingleton((Func<IServiceProvider, IConfigureOptions<TOptions>>)(p =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            var options = new ConfigureFromConfigurationOptions<TOptions>(configuration.GetSection(sectionKey));

            var settings = new TOptions();
            options.Action(settings);

            return options;
        }));

        return services;
    }
}
