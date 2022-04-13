using Azure.Core;
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
            string sectionKey = SectionKey)
        {
            services.AddOptions();
            services.ConfigureOptions<ConnectionStringOptions>(sectionKey);
            services.ConfigureOptions<Configuration.RetryOptions>(sectionKey);

            services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

            services.TryAddSingleton<IEventPublisher>(provider =>
            {
                var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
                var serviceBusClientProvider = new ConnectionStringProvider(
                    provider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>(),
                    provider.GetRequiredService<IOptionsMonitor<Configuration.RetryOptions>>()
                );

                return new ServiceBusPublisher(serviceBusClientProvider, eventNameResolver, serializer);
            });

            return services;
        }

        public static IServiceCollection AddErniAcademyTokenCredentialServiceBus(this IServiceCollection services,
            TokenCredential tokenCredential,
            ISerializer serializer, 
            string sectionKey = SectionKey)
        {
            services.AddOptions();
            services.ConfigureOptions<FullyQualifiedNamespaceOptions>(sectionKey);
            services.ConfigureOptions<Configuration.RetryOptions>(sectionKey);

            services.TryAddSingleton<IEventNameResolver, EventNameResolver>();

            services.TryAddSingleton<IEventPublisher>(provider =>
            {
                var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();
                var serviceBusClientProvider = new TokenCredentialProvider(
                    provider.GetRequiredService<IOptionsMonitor<FullyQualifiedNamespaceOptions>>(),
                    tokenCredential,
                    provider.GetRequiredService<IOptionsMonitor<Configuration.RetryOptions>>()
                );

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
