using Azure.Storage.Queues;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.StorageQueues.ClientProvider;
using ErniAcademy.Serializers.Contracts;
using System.Collections.Concurrent;

namespace ErniAcademy.Events.StorageQueues;

public class StorageQueueSubscriber<TEvent> : IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    private readonly Lazy<QueueClient> _queueClientLazy;
    private readonly ISerializer _serializer;
    private bool _susbcribed = true;

    private readonly ConcurrentDictionary<string, Func<TEvent, Task>> _handlers;

    public StorageQueueSubscriber(
        IQueueClientProvider queueClientProvider, 
        IEventNameResolver eventNameResolver,
        ISerializer serializer)
    {
        _queueClientLazy = new Lazy<QueueClient>(() => {
           return queueClientProvider.GetClient(eventNameResolver.Resolve<TEvent>());
        });

        _serializer = serializer;

        _handlers = new ConcurrentDictionary<string, Func<TEvent, Task>>();
    }

    public void Subscribe(Func<TEvent, Task> handler) => _handlers.TryAdd(handler.GetType().FullName, handler);

    public void UnSubscribe(Func<TEvent, Task> handler) => _handlers.TryRemove(handler.GetType().FullName, out Func<TEvent, Task> removed);

    public Task StarProcessingAsync(CancellationToken cancellationToken = default)
    {
        Task.Factory.StartNew(async () => {
            while (_susbcribed)
            {
                var queueMessages = await _queueClientLazy.Value.ReceiveMessagesAsync(cancellationToken);

                foreach (var message in queueMessages.Value)
                {
                    var @event = await _serializer.DeserializeFromStreamAsync<TEvent>(message.Body.ToStream(), cancellationToken);
                    foreach (var handler in _handlers)
                    {
                        await handler.Value.Invoke(@event);
                    }
                    await _queueClientLazy.Value.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
                }
            }
        }, TaskCreationOptions.LongRunning).ConfigureAwait(false);

        return Task.CompletedTask;
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken = default)
    {
        _susbcribed = false;
        return Task.CompletedTask;
    }
}
