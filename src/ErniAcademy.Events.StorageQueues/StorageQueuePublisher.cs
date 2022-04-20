using Azure.Storage.Queues;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.StorageQueues.ClientProvider;
using ErniAcademy.Serializers.Contracts;
using System.Collections.Concurrent;

namespace ErniAcademy.Events.StorageQueues;

public class StorageQueuePublisher : IEventPublisher
{
    private readonly IQueueClientProvider _queueClientProvider;
    private readonly IEventNameResolver _eventNameResolver;
    private readonly ISerializer _serializer;

    private readonly ConcurrentDictionary<string, QueueClient> _clients;

    public StorageQueuePublisher(
        IQueueClientProvider queueClientProvider, 
        IEventNameResolver eventNameResolver,
        ISerializer serializer)
    {
        _queueClientProvider = queueClientProvider;
        _eventNameResolver = eventNameResolver;
        _serializer = serializer;

        _clients = new ConcurrentDictionary<string, QueueClient>();
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent, new()
    {
        var client = GetClient(_eventNameResolver.Resolve<TEvent>());

        var message = await BuildMessage(@event);
        await client.SendMessageAsync(message, visibilityTimeout: null, timeToLive: null, cancellationToken);
    }

    public async Task PublishAsync<TEvent>(TEvent[] events,CancellationToken cancellationToken = default)
        where TEvent : class, IEvent, new()
    {
        foreach (var @event in events)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }

    protected virtual QueueClient GetClient(string queueName) => _clients.GetOrAdd(queueName, s => _queueClientProvider.GetClient(queueName));

    internal async Task<BinaryData> BuildMessage<TEvent>(TEvent @event) 
        where TEvent : class, IEvent, new()
    {
        await using var stream = new MemoryStream();
        await _serializer.SerializeToStreamAsync(@event, stream);

        var result = await BinaryData.FromStreamAsync(stream);
        return result;
    }
}
