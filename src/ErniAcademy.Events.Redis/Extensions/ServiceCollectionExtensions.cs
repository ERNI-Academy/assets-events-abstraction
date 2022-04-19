using ErniAcademy.Events.Contracts;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services,
        ISerializer serializer,
        string sectionKey)
    {
        services.AddOptions();
        services.ConfigureOptions<ConnectionStringOptions>(sectionKey);

        services.TryAddSingleton<IConnectionMultiplexerProvider, ConnectionMultiplexerProvider>();
        services.TryAddSingleton<IEventNameResolver, EventNameResolver>();
        
        services.TryAddSingleton<IEventPublisher>(provider =>
        {
            var connectionMultiplexerProvider = provider.GetRequiredService<IConnectionMultiplexerProvider>();
            var eventNameResolver = provider.GetRequiredService<IEventNameResolver>();

            return new RedisPublisher(connectionMultiplexerProvider, serializer, eventNameResolver);
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
