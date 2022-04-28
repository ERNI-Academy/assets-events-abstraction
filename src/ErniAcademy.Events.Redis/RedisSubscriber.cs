using ErniAcademy.Events.Contracts;
using ErniAcademy.Serializers.Contracts;
using StackExchange.Redis;

namespace ErniAcademy.Events.Redis;

public class RedisSubscriber<TEvent> : IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    private readonly Lazy<ChannelMessageQueue> _channelMessageQueueLazy;
    private readonly ISerializer _serializer;

    public RedisSubscriber(
        IConnectionMultiplexerProvider provider, 
        ISerializer serializer,
        IEventNameResolver eventNameResolver)
    {
        _serializer = serializer;
        
        _channelMessageQueueLazy = new Lazy<ChannelMessageQueue>(() => {
            var subscriber = provider.Connection.GetSubscriber();
            var redisChannel = new RedisChannel(eventNameResolver.Resolve<TEvent>(), RedisChannel.PatternMode.Auto);
            return subscriber.Subscribe(redisChannel);
        });
    }

    public event Func<TEvent, Task> ProcessEventAsync;

    public Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        _channelMessageQueueLazy.Value.OnMessage(message =>
        {
            var @event = _serializer.DeserializeFromString<TEvent>(message.Message.ToString());

            ProcessEventAsync.Invoke(@event);
        });

        return Task.CompletedTask;
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken = default) => _channelMessageQueueLazy.Value.UnsubscribeAsync();
}
