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

namespace ErniAcademy.Events.ServiceBus.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceBusFromConnectionString(this IServiceCollection services,
            ISerializer serializer,
            string sectionKey,
            ServiceBusClientOptions busOptions = null)
        {
            services.AddOptions();
            services.ConfigureOptions<ConnectionStringOptions>(sectionKey);

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
            TokenCredential tokenCredential,
            ISerializer serializer,
            string sectionKey,
            ServiceBusClientOptions busOptions = null)
        {
            services.AddOptions();
            services.ConfigureOptions<FullyQualifiedNamespaceOptions>(sectionKey);

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
}
