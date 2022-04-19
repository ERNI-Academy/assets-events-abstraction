using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.ProcessorProvider;
using ErniAcademy.Serializers.Contracts;
using System.Collections.Concurrent;

namespace ErniAcademy.Events.ServiceBus;

public class ServiceBusSubscriber<TEvent> : IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    private readonly ServiceBusProcessor _processor;
    private readonly ConcurrentDictionary<string, Func<TEvent, Task>> _handlers;
    private readonly ISerializer _serializer;

    public ServiceBusSubscriber(
        IServiceBusProcessorProvider serviceBusProcessorProvider, 
        IEventNameResolver eventNameResolver,
        ISerializer serializer)
    {
        var eventName = eventNameResolver.Resolve<TEvent>();

        _processor = serviceBusProcessorProvider.GetProcessor(eventName);
        _serializer = serializer;

        _handlers = new ConcurrentDictionary<string, Func<TEvent, Task>>();
    }

    public Task SubscribeAsync(Func<TEvent, Task> handler, CancellationToken cancellationToken = default)
    {
        _handlers.TryAdd(handler.GetType().FullName, handler);

        return Task.CompletedTask;
    }

    public Task UnSubscribeAsync(Func<TEvent, Task> handler, CancellationToken cancellationToken = default)
    {
        _handlers.TryRemove(handler.GetType().FullName, out Func<TEvent, Task> removed);

        return Task.CompletedTask;
    }

    public async Task StarProcessingAsync(CancellationToken cancellationToken = default)
    {
        _processor.ProcessMessageAsync += async args =>
        {
            var @event = await _serializer.DeserializeFromStreamAsync<TEvent>(args.Message.Body.ToStream(), cancellationToken);

            foreach (var handler in _handlers)
            {
                await handler.Value.Invoke(@event);
            }

            await args.CompleteMessageAsync(args.Message, cancellationToken);
        };

        _processor.ProcessErrorAsync += args => { throw args.Exception; };

        await _processor.StartProcessingAsync(cancellationToken);
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken = default) => _processor.StopProcessingAsync(cancellationToken);
}
