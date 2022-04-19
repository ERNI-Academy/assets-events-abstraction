using ErniAcademy.Events.Redis.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ErniAcademy.Events.Redis;

public class ConnectionMultiplexerProvider : IConnectionMultiplexerProvider
{
    private readonly Lazy<ConnectionMultiplexer> lazyConnection;
    public ConnectionMultiplexer Connection => lazyConnection.Value;

    public ConnectionMultiplexerProvider(IOptionsMonitor<ConnectionStringOptions> options)
    {
        lazyConnection = CreateConnection(options.CurrentValue.ConnectionString);
    }

    private static Lazy<ConnectionMultiplexer> CreateConnection(string connectionString) => new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
}
