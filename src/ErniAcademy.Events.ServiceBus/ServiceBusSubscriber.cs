using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.ProcessorProvider;
using ErniAcademy.Serializers.Contracts;

namespace ErniAcademy.Events.ServiceBus;

public class ServiceBusSubscriber<TEvent> : IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    private readonly ServiceBusProcessor _processor;
    private readonly ISerializer _serializer;

    public ServiceBusSubscriber(
        IServiceBusProcessorProvider serviceBusProcessorProvider, 
        IEventNameResolver eventNameResolver,
        ISerializer serializer)
    {
        var eventName = eventNameResolver.Resolve<TEvent>();

        _processor = serviceBusProcessorProvider.GetProcessor(eventName);
        _serializer = serializer;
    }

    public event Func<TEvent, Task> ProcessEventAsync;

    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        _processor.ProcessMessageAsync += async args =>
        {
            var @event = await _serializer.DeserializeFromStreamAsync<TEvent>(args.Message.Body.ToStream(), cancellationToken);

            await ProcessEventAsync.Invoke(@event);

            await args.CompleteMessageAsync(args.Message, cancellationToken);
        };

        _processor.ProcessErrorAsync += args =>
        {
            throw new Exception("ProcessErrorAsync", args.Exception);
        };

        await _processor.StartProcessingAsync(cancellationToken);
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken = default) => _processor.StopProcessingAsync(cancellationToken);
}
