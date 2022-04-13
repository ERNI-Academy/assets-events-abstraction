using Azure.Core;
using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.ClientProvider;
using ErniAcademy.Events.ServiceBus.Configuration;
using ErniAcademy.Events.ServiceBus.Serializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.ServiceBus.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const string SectionKey = "ErniAcademy:Events:ServiceBus";

        public static IServiceCollection AddErniAcademyConnectionStringServiceBus(this IServiceCollection services,
            ISerializer serializer,
            ServiceBusClientOptions busOptions = null,
            string sectionKey = SectionKey)
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

        public static IServiceCollection AddErniAcademyTokenCredentialServiceBus(this IServiceCollection services,
            TokenCredential tokenCredential,
            ISerializer serializer,
            ServiceBusClientOptions busOptions = null,
            string sectionKey = SectionKey)
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

        internal static IServiceCollection ConfigureOptions<TOptions>(this IServiceCollection services, string sectionKey = SectionKey)
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
