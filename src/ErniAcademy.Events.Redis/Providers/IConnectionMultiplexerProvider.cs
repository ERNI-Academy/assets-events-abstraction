using StackExchange.Redis;

namespace ErniAcademy.Events.Redis;

public interface IConnectionMultiplexerProvider
{
    public ConnectionMultiplexer Connection { get; }
}
