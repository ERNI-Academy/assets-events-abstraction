using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.Redis.Configuration;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ErniAcademy.Events.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services,
        IConfiguration configuration,
        ISerializer serializer,
        string sectionKey)
    {
        services.AddOptions<ConnectionStringOptions>().Bind(configuration.GetSection(sectionKey)).ValidateDataAnnotations();

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
}
