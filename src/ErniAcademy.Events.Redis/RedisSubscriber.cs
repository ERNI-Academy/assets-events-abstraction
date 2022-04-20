using ErniAcademy.Events.Contracts;
using ErniAcademy.Serializers.Contracts;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace ErniAcademy.Events.Redis;

public class RedisSubscriber<TEvent> : IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    private readonly Lazy<ChannelMessageQueue> _channelMessageQueueLazy;
    private readonly ISerializer _serializer;
    private readonly ConcurrentDictionary<string, Func<TEvent, Task>> _handlers;

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

        _handlers = new ConcurrentDictionary<string, Func<TEvent, Task>>();
    }

    public void Subscribe(Func<TEvent, Task> handler) => _handlers.TryAdd(handler.GetType().FullName, handler);

    public void UnSubscribe(Func<TEvent, Task> handler) => _handlers.TryRemove(handler.GetType().FullName, out Func<TEvent, Task> removed);

    public Task StarProcessingAsync(CancellationToken cancellationToken = default)
    {
        _channelMessageQueueLazy.Value.OnMessage(message =>
        {
            var @event = _serializer.DeserializeFromString<TEvent>(message.Message.ToString());

            foreach (var handler in _handlers)
            {
                handler.Value.Invoke(@event).GetAwaiter().GetResult();
            }
        });

        return Task.CompletedTask;
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken = default) => _channelMessageQueueLazy.Value.UnsubscribeAsync();
}
