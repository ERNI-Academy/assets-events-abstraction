using ErniAcademy.Events.Contracts;
using ErniAcademy.Serializers.Contracts;
using StackExchange.Redis;

namespace ErniAcademy.Events.Redis;

public class RedisPublisher : IEventPublisher
{
    private readonly Lazy<ISubscriber> _subscriberLazy;
    private ISubscriber _subscriber => _subscriberLazy.Value;

    private readonly ISerializer _serializer;
    private readonly IEventNameResolver _eventNameResolver;

    public RedisPublisher(
        IConnectionMultiplexerProvider provider, 
        ISerializer serializer,
        IEventNameResolver eventNameResolver)
    {
        _serializer = serializer;
        _eventNameResolver = eventNameResolver;
        _subscriberLazy = new Lazy<ISubscriber>(provider.Connection.GetSubscriber());
    }

    public Task PublishAsync<TEvent>(TEvent @event) 
        where TEvent : class, IEvent, new()
    {
        var redisChannel = new RedisChannel(_eventNameResolver.Resolve(@event), RedisChannel.PatternMode.Auto);
        var redisValue = new RedisValue(_serializer.SerializeToString(@event));
        return _subscriber.PublishAsync(redisChannel, redisValue, CommandFlags.FireAndForget);
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : class, IEvent, new()
    {
        var redisChannel = new RedisChannel(_eventNameResolver.Resolve(@event), RedisChannel.PatternMode.Auto);
        var redisValue = new RedisValue(_serializer.SerializeToString(@event));
        return _subscriber.PublishAsync(redisChannel, redisValue, CommandFlags.FireAndForget);
    }

    public Task PublishAsync<TEvent>(TEvent[] events, CancellationToken cancellationToken)
    {
        foreach (var @event in events)
        {

        }
    }
}
